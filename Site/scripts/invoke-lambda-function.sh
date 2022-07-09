#!/usr/bin/env bash

# From code/package-and-deploy directory run ./scripts/invoke-lambda-function.sh <TIME_ZONE>

LAMBDA_FUNCTION_NAME="fn1-prod"
PAYLOAD=""

aws lambda invoke --function-name "${LAMBDA_FUNCTION_NAME}" --payload "${PAYLOAD}" response.json
cat response.json
