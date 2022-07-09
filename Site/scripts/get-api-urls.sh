#!/usr/bin/env bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

function getGatewayUriForStack() {
  local stackName=$1
  aws cloudformation describe-stacks --stack-name "${stackName}" | jq -r '.Stacks[].Outputs[] | select(.OutputKey=="GatewayUri") | .OutputValue'
}

echo pre-prod
getGatewayUriForStack "cfn-msfs-package-deploy-pre-prod"
echo prod
getGatewayUriForStack "cfn-msfs-package-deploy-prod"
