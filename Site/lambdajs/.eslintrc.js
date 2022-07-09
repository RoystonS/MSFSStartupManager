const path = require('path');

module.exports = {
  root: true,
  extends: ['eslint:recommended', 'plugin:@typescript-eslint/recommended'],
  parser: '@typescript-eslint/parser',
  parserOptions: { project: [path.join(__dirname, 'tsconfig.json')] },
  plugins: ['@typescript-eslint'],
  rules: {
    '@typescript-eslint/explicit-member-accessibility': 'error',
    '@typescript-eslint/no-non-null-assertion': 'off',
    'no-constant-condition': 'off',
    'no-unused-vars': 'off',
  },
  ignore: '.eslintrc.js',
};
