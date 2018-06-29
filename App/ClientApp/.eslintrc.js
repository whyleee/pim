module.exports = {
  root: true,
  env: {
    node: true
  },
  'extends': [
    'plugin:vue/recommended',
    '@vue/airbnb'
  ],
  rules: {
    'no-console': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',

    // airbnb base overrides
    'comma-dangle': ['error', 'never'], // trailing commas is too much
    'eqeqeq': 'off', // too strict, == is ok almost in all cases
    'semi': ['error', 'never'], // you don't need semis
  },
  parserOptions: {
    parser: 'babel-eslint'
  }
}
