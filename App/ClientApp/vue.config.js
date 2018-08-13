module.exports = {
  devServer: {
    proxy: 'https://localhost:44374'
  },
  configureWebpack: {
    devtool: 'source-map'
  }
}
