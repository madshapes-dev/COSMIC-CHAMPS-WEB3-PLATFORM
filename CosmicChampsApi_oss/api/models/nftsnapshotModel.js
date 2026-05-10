'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;


const HolderSchema = new Schema({
    _id: String,
    count: Number,
});

//this will keep snapshot for EACH individual NFT/skin - simplest like that
//handle actual cron for this by rarity to split the batches
var NftSnapshotSchema = new Schema({
  _id: String,
  rarity:String,
  base:String,
  name:String,
  updated_date: {
    type: Date,
    default: Date.now
  },
  holders:[HolderSchema],
});

module.exports = mongoose.model('NftSnapshots', NftSnapshotSchema);