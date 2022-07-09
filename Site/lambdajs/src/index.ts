import type {
  APIGatewayProxyEventV2,
  APIGatewayProxyHandlerV2,
  APIGatewayProxyResultV2,
  APIGatewayProxyStructuredResultV2,
} from 'aws-lambda';

import { getSignedUrl } from '@aws-sdk/s3-request-presigner';
import { PutObjectCommand } from '@aws-sdk/client-s3';
import { BucketName } from './env';
import { s3Client } from './awsClients';

type RequestInfo = Pick<
  APIGatewayProxyEventV2,
  'rawPath' | 'rawQueryString' | 'body'
>;

interface StatusResponse {
  supported: boolean;
  requiredVersion: string;
}

interface UploadRequest {
  size: number;
}

interface UploadResponse {
  url: string;
}

async function handleGet(request: RequestInfo) {
  let result: APIGatewayProxyStructuredResultV2;

  if (request.rawPath === '/status') {
    const response: StatusResponse = {
      supported: true,
      requiredVersion: '0.1.1',
    };
    result = {
      statusCode: 200,
      body: JSON.stringify(response),
    };
  } else {
    result = {
      statusCode: 404,
    };
  }
  return result;
}

async function handlePost(request: RequestInfo) {
  let result: APIGatewayProxyResultV2 = {
    statusCode: 404,
    body: request.rawPath,
  };

  if (request.rawPath === '/upload') {
    const uploadRequest = JSON.parse(request.body ?? '{}') as UploadRequest;
    const uploadSize = uploadRequest.size ?? 0;

    if (uploadSize < 50 || uploadSize > 100000) {
      result = {
        statusCode: 400,
      };
    } else {
      const command = new PutObjectCommand({
        Bucket: BucketName,
        Key: new Date().toISOString()+".zip",
        ContentLength: uploadSize
      });

      console.log(
        `Generating URL for uploading ${uploadSize} to`,
        command.input.Bucket,
        command.input.Key
      );

      const url = await getSignedUrl(s3Client, command, {
        expiresIn: 300,
      });

      const response: UploadResponse = {
        url,
      };
      result = {
        statusCode: 200,
        body: JSON.stringify(response),
      };
    }
  }
  return result;
}

const handler: APIGatewayProxyHandlerV2 = async (event, context) => {
  const method = event.requestContext.http.method;

  console.log(`method`, method);
  console.log(`event`, JSON.stringify(event));

  switch (method) {
    case 'GET':
      return await handleGet(event);
    case 'POST':
      return await handlePost(event);
  }

  return {
    statusCode: 404,
  };
};

export { handler };

/*
let stuff = {
  status: 'OK',
  event: {
    version: '2.0',
    routeKey: '$default',
    rawPath: '/test',
    rawQueryString: 'abc',
    headers: {
      accept:
        'text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*;q=0.8,application/signed-exchange;v=b3;q=0.9',
      'accept-encoding': 'gzip, deflate, br',
      'accept-language': 'en-GB;q=0.9',
      'content-length': '0',
      host: '0d9qt2j4x6.execute-api.eu-west-2.amazonaws.com',
      'sec-fetch-dest': 'document',
      'sec-fetch-mode': 'navigate',
      'sec-fetch-site': 'none',
      'sec-fetch-user': '?1',
      'sec-gpc': '1',
      'upgrade-insecure-requests': '1',
      'user-agent':
        'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.61 Safari/537.36',
      'x-amzn-trace-id': 'Root=1-6295528d-640a4879230487c20b2f28a7',
      'x-forwarded-for': '81.151.41.220',
      'x-forwarded-port': '443',
      'x-forwarded-proto': 'https',
    },
    queryStringParameters: { abc: '' },
    requestContext: {
      accountId: '847952955188',
      apiId: '0d9qt2j4x6',
      domainName: '0d9qt2j4x6.execute-api.eu-west-2.amazonaws.com',
      domainPrefix: '0d9qt2j4x6',
      http: {
        method: 'GET',
        path: '/test',
        protocol: 'HTTP/1.1',
        sourceIp: '81.151.41.220',
        userAgent:
          'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.61 Safari/537.36',
      },
      requestId: 'S9nWIhDWLPEEJcw=',
      routeKey: '$default',
      stage: '$default',
      time: '30/May/2022:23:26:05 +0000',
      timeEpoch: 1653953165435,
    },
    isBase64Encoded: false,
  },
};
*/
