export default {
  backends: [
    {
      key: 'products',
      title: 'Products',
      meta: {
        url: '/api/meta'
      },
      data: {
        url: '/api/products'
      }
    },
    {
      key: 'tags',
      title: 'Tags',
      meta: {
        url: 'http://localhost:63731/v0/meta'
      },
      data: {
        url: 'http://localhost:63731/v0/instances',
        collectionItemsProperty: 'items'
      }
    }
  ]
}