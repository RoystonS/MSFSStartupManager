export const AwsRegion = getEnvironmentVariable('AWS_REGION');

export const BucketName = getEnvironmentVariable('BUCKET_NAME');

function getEnvironmentVariable(
  name: string,
  defaultValue?: string | undefined
) {
  const value = process.env[name];
  if (typeof defaultValue === 'undefined' && typeof value === 'undefined') {
    throw new Error(`Missing environment variable: ${name}`);
  }

  return (value ?? defaultValue)!;
}
