const path = require('path')

module.exports = {
  root: true,
  env: {
    node: true
  },
  settings: {
    'import/resolver': {
      webpack: {
        config: {
          resolve: {
            alias: {
              '@': path.resolve(__dirname, 'src')
            }
          }
        }
      }
    }
  },
  extends: [
    'plugin:vue/recommended',
    'plugin:import/recommended',
    'plugin:import/errors',
    'plugin:import/warnings',
    '@vue/airbnb'
  ],
  rules: {
    'no-console': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',

    // airbnb base overrides
    'comma-dangle': ['error', 'never'], // trailing commas is too much
    'eqeqeq': 'off', // too strict, == is ok almost in all cases
    'linebreak-style': 'off', // lf doesn't work in vs, git autocrlf=true handles this
    'no-param-reassign': ['error', { 'props': false }], // modifying props is ok, especially in reduce
    'semi': ['error', 'never'] // you don't need semis
  },
  parserOptions: {
    ecmaVersion: 2018,
    parser: 'babel-eslint'
  }
}
