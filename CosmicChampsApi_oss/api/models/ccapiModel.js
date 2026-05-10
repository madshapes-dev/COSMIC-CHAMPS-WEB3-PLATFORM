'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;


var WalletSchema = new Schema({
  _id:String,
  name: {
    type: String
  },
  status: {
    type: Boolean,
    default: false
  },
  updated_date: {
    type: Date,
    default: Date.now
  },

});

module.exports = mongoose.model('Wallets', WalletSchema);