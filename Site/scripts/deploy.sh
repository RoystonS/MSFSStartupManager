#!/usr/bin/env bash

CLOUD_ENV=$1
shift

if [ "${CLOUD_ENV}" = "" ]; then
  echo Specify environment pre-prod or prod
  exit 5
fi
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)

function reportWarning() {
  echo -e "\033[0;33m${1}\033[0m"
}

function reportFailureAndExit() {
  echo -e "\033[0;31m${1}\033[0m"
  exit 5
}

function rawurlencode() {
  local string="${1}"
  local strlen=${#string}
  local encoded=""
  local pos c o

  for ((pos = 0; pos < strlen; pos++)); do
    c=${string:$pos:1}
    case "$c" in
    [-_.~a-zA-Z0-9]) o="${c}" ;;
    *) printf -v o '%%%02x' "'$c" ;;
    esac
    encoded+="${o}"
  done
  echo "${encoded}"
}

INFRASTRUCTURE_BUCKET_NAME=cfn-lambda-nodejs
AWS_REGION=eu-west-2
STACK_NAME=cfn-msfs-package-deploy-${CLOUD_ENV}
YEAR_MONTH=$(date +%Y-%m)

cd "${SCRIPT_DIR}/.." || exit

# echo Compiling NodeJS function
# pushd lambdajs || exit
# rm -rf dist
# if ! npm install; then
#   reportFailureAndExit "npm installation failed"
# fi
# if ! npm run build:production; then
#   reportFailureAndExit "npm build failed"
# fi
# popd || exit

# Upload any files referenced from the template
if ! aws cloudformation package --template-file ./infrastructure.template.yaml --s3-bucket "${INFRASTRUCTURE_BUCKET_NAME}" --s3-prefix "${STACK_NAME}/${YEAR_MONTH}" --output-template-file ./infrastructure-packaged.template.yaml; then
  reportFailureAndExit "Could not create deployment package"
fi

echo "Validating"
if ! aws cloudformation validate-template --template-body file://infrastructure-packaged.template.yaml; then
  reportFailureAndExit "Template validation failed"
fi

CHANGE_SET_NAME=${STACK_NAME}-change

# Delete old change set with the same name
aws cloudformation delete-change-set --change-set-name "${CHANGE_SET_NAME}" --stack-name "${STACK_NAME}" --region "${AWS_REGION}" >/dev/null 2>/dev/null

echo Creating change set
change_set_id=$(aws cloudformation create-change-set --change-set-name "${CHANGE_SET_NAME}" --template-body file://infrastructure-packaged.template.yaml --stack-name "${STACK_NAME}" --region "${AWS_REGION}" --capabilities CAPABILITY_IAM --parameters ParameterKey=Environment,ParameterValue="${CLOUD_ENV}" --output json | jq -r '.Id')
if [[ -n "${change_set_id}" ]]; then
  stack_id=$(aws cloudformation describe-change-set --change-set-name "${change_set_id}" --output json | jq -r '.StackId')

  encoded_stack_id=$(rawurlencode "${stack_id}")
  encoded_change_set_id=$(rawurlencode "${change_set_id}")

  echo "https://${AWS_REGION}.console.aws.amazon.com/cloudformation/home?region=${AWS_REGION}#/stacks/changesets/changes?stackId=${encoded_stack_id}&changeSetId=${encoded_change_set_id}"

  if aws cloudformation wait change-set-create-complete --change-set-name "${change_set_id}"; then
    echo Creation complete. Please review and deploy.
  else
    reportWarning "$(aws cloudformation describe-change-set --change-set-name "${change_set_id}" --output json | jq -r '.StatusReason')"
    reportFailureAndExit "Change set failed validation"
  fi
else
  reportFailureAndExit "Failed to create deployment change set"
fi
