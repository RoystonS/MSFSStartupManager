import { S3Client } from '@aws-sdk/client-s3';

import { AwsRegion } from './env';

const s3Client = new S3Client({ region: AwsRegion });
export { s3Client };
