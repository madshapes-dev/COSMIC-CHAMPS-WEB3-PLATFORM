'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;


var MatchSchema = new Schema({
  _id:String,
  start_date: {
    type: Date,
    default: Date.now
  },
  playera:String,
  playerb:String,
  winner:String,
  end_date:Date,
});

module.exports = mongoose.model('Matches', MatchSchema);