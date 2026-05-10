'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;


const SkinSchema = new Schema({
    _id: String,
    count: Number,
});


var InventorySchema = new Schema({
  _id:String,
  updated_date: {
    type: Date,
    default: Date.now
  },
  coyote:{
    skins:[SkinSchema],
  },
  cybi:{
    skins:[SkinSchema],
  },
  hammer:{
    skins:[SkinSchema],
  },
  helio:{
    skins:[SkinSchema],
  },
  invin:{
    skins:[SkinSchema],
  },
  ram:{
    skins:[SkinSchema],
  },
  tertius:{
    skins:[SkinSchema],
  },
  trig:{
    skins:[SkinSchema],
  },
});

module.exports = mongoose.model('Inventories', InventorySchema);