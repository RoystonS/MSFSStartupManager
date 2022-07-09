#!/usr/bin/env bash

CLOUD_ENV=$1
shift

if [ "${CLOUD_ENV}" = "" ]; then
  echo Specify environment pre-prod or prod
  exit 5
fi
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)

function reportFailureAndExit() {
  echo -e "\033[0;31m${1}\033[0m"
  exit 5
}

INFRASTRUCTURE_BUCKET_NAME=cfn-lambda-nodejs
AWS_REGION=eu-west-2
STACK_NAME=cfn-msfs-package-deploy-${CLOUD_ENV}
YEAR_MONTH=$(date +%Y-%m)

cd "${SCRIPT_DIR}/.." || exit

echo Compiling NodeJS function
pushd lambdajs || exit
rm -rf dist
if ! npm install; then
  reportFailureAndExit "npm installation failed"
fi
if ! npm run build:production; then
  reportFailureAndExit "npm build failed"
fi
popd || exit

# Upload any files referenced from the template
if ! aws cloudformation package --template-file ./infrastructure.template.yaml --s3-bucket "${INFRASTRUCTURE_BUCKET_NAME}" --s3-prefix "${STACK_NAME}/${YEAR_MONTH}" --output-template-file ./infrastructure-packaged.template.yaml; then
  reportFailureAndExit "Could not create deployment package"
fi

echo "Validating"
if ! aws cloudformation validate-template --template-body file://infrastructure-packaged.template.yaml; then
  reportFailureAndExit "Template validation failed"
fi

echo "Deploying ${STACK_NAME}"
if aws cloudformation deploy --template-file ./infrastructure-packaged.template.yaml --stack-name "${STACK_NAME}" --region "${AWS_REGION}" --capabilities CAPABILITY_IAM --parameter-overrides Environment="${CLOUD_ENV}"; then
  echo -e "\033[0;33mDeployment succeeded\033[0m"
  aws cloudformation describe-stacks --stack-name "${STACK_NAME}" | jq '.Stacks | .[].Outputs'
else
  aws cloudformation describe-stack-events --stack-name "${STACK_NAME}"
  reportFailureAndExit "Deployment failed"
fi
