
const BOOSTS = {
    spacerock : {
        /* leggy */
        "1":5, /* diamond tier spacerock_1*/
        "2":3, /* gold tier spacerock_2 */
        "3":2, /* silver tier spacerock_3*/
        "4":1, /* bronze tier spacerock_4*/
    },

    blade : {
        /* leggy */
     "798555187":5, /*  Lava Blade */
     "798555943":5, /* Angel Blade */
     "798557320":5, /* Kitty Blade */
     "833035531":5, /* Apricot Blade */
     "833035647":5, /* Ice Blade */
     "833035774":5, /* Ghost Blade*/
    
        /* epic */
     "798555469":3, /* Night Blade" */
     "798556286":3, /* Impala Blade  */
     "798556445":3, /* Trippy Blade" */
     "798557146":3, /* X Blade  */
     "833035423":3, /* Wolf Blade" */
    
    /* rare */
    "798555307":2, /* Tiger Blade" */
    "798555618":2, /* Warp Blade" */
    "798556080":2, /* Shimmer Blade" */
    "798556623":2, /* Tri Blade" */
    "798556817":2, /* Walker Blade" */
    "798556984":2, /* Moon Blade" */
    
    },

    hummingbird : {
        /* leggy */
        "833312776":5, /*  Elixir Hummingbird */
        "833312920":5, /* Nectar Hummingbird */
        "833313073":5, /* Deathless Hummingbird */
        "833313200":5,/* Immortality Hummingbird */
        "833313356":5,/* Paradise Hummingbird */
        "833313471":5,/* Poison Hummingbird*/
       
           /* epic */
        "833312060":3, /* Amrita Hummingbird" */
        "833312247":3, /* Melon Hummingbird  */
        "833312366":3, /* Tonic Hummingbird" */
        "833312496":3, /* Venom Hummingbird  */
        "833312632":3, /* Acid Hummingbird" */
       
       /* rare */
       "833311075":2, /* Potion Hummingbird" */
       "833311241":2, /* Toxin Hummingbird" */
       "833311404":2, /* Velvet Hummingbird" */
       "833311555":2, /* Ophidian Hummingbird" */
       "833311723":2, /* Honey Hummingbird" */
       "833311879":2, /* Arsenic Hummingbird" */
    
    },
    
    cybi : {
        /* leggy */
     "786130175":13, /*Cybi 1* */
     "786130476":13, /*cybi 2* */
     "786130739":13, /*Cybi 3* */
     "798403857":13,/*Mycenae Cybi*/
     "798404217":13,/*Exosphere Cybi*/
     "798404652":13,/*Lumbermaster Cybi*/
    
        /* epic */
     "786130313":7, /* Cybi 1 */
     "798403598":7, /* Auric Cybi  */
    
        /* rare */
     "786130883":4, /* Cybi 3  */
     "798404024":4, /* Carbon Cybi  */
     "798404492":4, /* Lumberjack Cybi  */
    
        /*uncommon */
     "786130582":2, /* Cybi 2  */
    
        /* special */
     "981349173":1, /* Red Chroma Cybi  */
     "2395662892":1, /* Cauldron Witch Cybi  */
    
    },


    tertius : {
        /* leggy */
     "786131096":13, /*  Tertius 3 1* */
     "786131403":13, /* Tertius 3 2* */
     "786131696":13, /* Tertius 3 3* */
     "798407177":13,/* Error */
     "798407610":13,/* Dolor */
     "798408091":13,/* Frozenstar*/
    
        /* epic */
     "786131264":7, /* Tertius 3 1 */
     "798407875":7, /* Jadestar  */
    
        /* rare */
     "786131555":4, /* Tertius 3 2  */
     "798407031":4, /* Warning  */
     "798407410":4, /* Kitty  */
    
        /*uncommon */
     "786131874":2, /* Tertius 3 3  */
    
    },
    
    
    coyote : {
        /* leggy */
     "786224106":13, /*  Coyote 1* */
     "787412653":13, /* Coyote 3* */
     "798402169":13, /* Molten Coyote */
     "798402784":13,/* Antiplanet Coyote */
     "798403096":13,/* StringTheory Coyote */
     "798403436":13,/* SpiralCandy Coyote */
    
        /* epic */
     "786224391":7, /* Coyote 1 */
     "798403287":7, /* Hardboiled Coyote  */
    
        /* rare */
     "798402012":4, /* Darkmatter Coyote  */
     "798402625":4, /* Fury Coyote  */
     "798402938":4, /* Wavelength Coyote  */
    
        /*uncommon */
     "786224565":2, /* Coyote 3  */
    
    },
    
    
    hammer : {
        /* leggy */
     "798408523":13, /*  Torus Hammer */
     "798408945":13, /* Frozenstar Hammer */
     "798409333":13, /* Coco Hammer */
     "798409700":13,/* Shark Hammer */
     "798410021":13,/* Freebooter Hammer */
     "798410307":13,/* Galileo Hammer*/
    
        /* epic */
     "798408294":7, /* Steampink Hammer */
     "798408694":7, /* DarkIce Hammer  */
    
        /* rare */
     "798409130":4, /* Kitty Hammer  */
     "798409552":4, /* Bomber Hammer  */
     "798409863":4, /* RedDwarf Hammer  */
    
        /*uncommon */
     "798410164":2, /* Exoplanet Hammer  */
    
    },


    ram : {
        /* leggy */
     "798412077":13, /*  Dragonkite Ram */
     "798412449":13, /* Roadracer Ram */
     "798412788":13, /* Dazzle Ram */
     "798413075":13,/* Mahogany Ram */
     "798413364":13,/* Treasure Ram */
     "798413704":13,/* Glimmer Ram*/
     "1108378539":13,/* Moonshot Ram */
     "1280977839":13, /*ram & golden mngo */
    
        /* epic */
     "798411946":7, /* Dragonfly Ram */
     "798412284":7, /* TrackCrew Ram  */
    
        /* rare */
     "798412603":4, /* Flicker Ram  */
     "798412919":4, /* BattleAxe Ram  */
     "798413212":4, /* Quest Ram  */
    
        /*uncommon */
     "798413518":2, /* Shimmer Ram  */
    
        /* special */
     "1108380528":1, /* PP Ram  */
     "1280977773":1, /**ram & mngo */
    
    },
    
    
    helio : {
        /* leggy */
     "798414063":13, /*  Metallicity Helio */
     "798414425":13, /* Dirtsider Helio */
     "798414797":13, /* Geospace Helio */
     "798415137":13,/* Eclipse Helio */
     "798415432":13,/* Filament Helio */
     "798415766":13,/* Fort Helio*/
    
        /* epic */
     "798413845":7, /* Stargazer Helio */
     "798414243":7, /* Planetfall Helio  */
    
        /* rare */
     "798414597":4, /* Astrophile Helio  */
     "798414953":4, /* Isotropy Helio  */
     "798415284":4, /* Bubble Helio  */
    
        /*uncommon */
     "798415616":2, /* Tidal Helio  */
    
    },
    
    trig : {
        /* leggy */
     "798473467":13, /*  GateGeek Trig */
     "798473794":13, /* Academy Trig */
     "798474101":13, /* Pisces Trig */
     "798474468":13,/* Sopdet Trig */
     "815278156":13,/* Neo Trig */
     "815281301":13,/* Cosmonaut Trig*/
    
        /* epic */
     "798473335":7, /* Atlantean Trig */
     "815281236":7, /* Odyssey Trig  */
    
        /* rare */
     "798473623":4, /* Epoch Trig  */
     "798473949":4, /* Lacerta Trig  */
     "815278091":4, /* Morph Trig  */
    
        /*uncommon */
     "798474292":2, /* Pharaoh Trig  */
    
    },
    
    
    invin : {
        /* leggy */
     "815316306":13, /*  Hypnodot Invin */
     "815316472":13, /* Steampunk Invin */
     "815316629":13, /* Goldilocks Invin */
     "815316767":13,/* Captain Invin */
     "815316935":13,/* Candle Invin */
     "815317076":13,/* Astro Invin*/
    
        /* epic */
     "815316384":7, /* Steampink Invin */
     "815316558":7, /* Prime Invin  */
    
        /* rare */
     "815316700":4, /* Pirate Invin  */
     "815316865":4, /* Bear Invin  */
     "815317022":4, /* Plasma Invin  */
    
        /*uncommon */
     "798485160":2, /* Polkadot Invin  */
    
    },
    
    hawk : {
        /* leggy */
     "1014251400":13, /*  Punk Hawk */
     "1014251100":13, /* Sentinel Hawk */
     "1014250827":13, /* Raptor Hawk */
     "1014250577":13,/* Swerve Hawk */
     "1014250204":13,/* Techni Hawk */
     "1014248680":13,/* Galacto Hawk */
     "1014247938":13,/* Humm Hawk */
    
        /* epic */
     "1014251258":7, /* Skybound Hawk */
     "1014250950":7, /* Assasin Hawk  */
    
        /* rare */
     "1014250675":4, /* Azure Hawk */
     "1014250478":4, /* Peak Hawk */
     "1014249155":4, /* Grackle Hawk */
     "1014248536":4, /* Gully Hawk */
     "1014247514":4, /* Holo Hawk */
    
        /*uncommon */
     "1014246973":2, /* Soak Hawk */
    
        /* common */
     "1014246764":1, /* Strike Hawk */
    
    },
    
    
    pixie : {
        /* leggy */
     "1014108850":13, /* Inkwell Pix  */
     "1014108682":13, /* Charged Pix  */
     "1014108564":13, /* Bluebell Pix  */
     "1014108397":13,/* Gemstone Pix  */
     "1014108293":13,/* Typhoon Pix */
     "1014108163":13,/* Navigator Pix  */
     "1014108051":13,/* Tailwind Pix  */
    
        /* epic */
     "1014108747":7, /* Inkling Pix  */
     "1014108631":7, /* Neon Pix  */
    
        /* rare */
     "1014108513":4, /* Foxglove Pix */
     "1014108344":4, /* Bubblegum Pix */
     "1014108208":4, /* Enigma Pix */
     "1014108117":4, /* Emergence Pix */
     "1014107923":4, /* Dragonfly Pix */
    
        /*uncommon */
     "1014107806":2, /* Motherboard Pix */
    
        /* common */
     "1014107768":1, /* Glider Pix */
    
    },
    
    
    shockray : {
        /* leggy */
        "1013972790":13, /* Mech Shockray */
        "1013972611":13, /* Salamander Shockray */
        "1013972480":13, /* Unleashed Shockray */
        "1013972328":13,/* Lagoon Shockray */
        "1013972218":13,/* Ebby Shockray */
        "1013972027":13,/* Snowy Shockray */
        "1013971823":13,/* Ember Shockray */
       
           /* epic */
        "1013972704":7, /* Wave Shockray */
        "1013972556":7, /* Saurus Shockray */
       
           /* rare */
        "1013972389":4, /* Glitchy Shockray */
        "1013972261":4, /* Starlight Shockray */
        "1013972156":4, /* Eddy Shockray */
        "1013971908":4, /* Icy Shockray */
        "1013971732":4, /* Scorched Shockray */
       
           /*uncommon */
        "1013971635":2, /* Clown Shockray */
       
           /* common */
        "1013971535":1, /* Arc Shockray  */
    
    },

    cyclops : {
        /* leggy */
        "1013855418":13, /* Phoenix Cyclops */
        "1013855278":13, /* Lava Cyclops */
        "1013855149":13, /* Viz Cyclops */
        "1013855004":13,/* Fusion Cyclops */
        "1013854854":13,/* Astral Cyclops */
        "1013854704":13,/* Tempest Cyclops */
        "1013854270":13,/* Aegis Cyclops */
       
           /* epic */
        "1013855359":7, /* Shadow Cyclops */
        "1013855199":7, /* Deepwell Cyclops */
       
           /* rare */
        "1013855078":4, /* Catcher Cyclops */
        "1013854931":4, /* Watcher Cyclops */
        "1013854799":4, /* Swirl Cyclops */
        "1013854342":4, /* Acid Cyclops */
        "1013854198":4, /* Atom Cyclops */
       
           /*uncommon */
        "1013854132":2, /* Lumi Cyclops */
       
           /* common */
        "1013854047":1, /* Phantom Cyclops */
    
    },
    
       
    jax : {
        /* leggy */
        "1013537574":13, /* Inferno Jax */
        "1013537455":13, /* Smackdown Jax */
        "1013537346":13, /* Knockout Jax */
        "1013537236":13,/* Scrum Jax */
        "1013537078":13,/* Insanity Jax */
        "1013536955":13,/* Stellar Jax */
        "1013536794":13,/* Photon Jax */
       
           /* epic */
        "1013537506":7, /* Ironfist Jax */
        "1013537405":7, /* Showdown Jax */
       
           /* rare */
        "1013537283":4, /* Gauntlet Jax */
        "1013537183":4, /* Sonic Jax */
        "1013537023":4, /* Slugfest Jax */
        "1013536882":4, /* Quantum Jax */
        "1013535601":4, /* Hyperion Jax */
       
           /*uncommon */
        "1013535528":2, /* Lumberjack Jax  */
       
           /* common */
        "1013535480":1, /* Punchdown Jax */
    
    },
    
      
    bff : {
        /* leggy */
        "1013353488":13, /* Alligator BFF  */
        "1013353336":13, /* Sphinx BFF */
        "1013353222":13, /* Gecko BFF */
        "1013353055":13,/* Ember BFF */
        "1013352851":13,/* Toxic BFF */
        "1013352720":13,/* Valley BFF */
        "1013352565":13,/* Doom BFF */
       
           /* epic */
        "1013353395":7, /* Anaconda BFF */
        "1013353281":7, /* Saurian BFF */
       
           /* rare */
        "1013353154":4, /* Dragon BFF */
        "1013352971":4, /* Starlight BFF */
        "1013352778":4, /* Molten BFF */
        "1013352643":4, /* Meadow BFF */
        "1013352483":4, /* Fiery BFF */
       
           /*uncommon */
        "1013352421":2, /* Sandblaster BFF */
       
           /* common */
        "1013352360":1, /* Oasis BFF */
    
    },

    striker : {
      /*special*/
      "2230768437":1, /*summer goanna striker */
      "2230768537":1,  /* morningstar goanna stirker */
    },

    fireball : {
      /* leggy */
      /*"2411818661":13,*/ /*  Ultimate fireball */
      "700000000":13,
         /* epic */
      /*"2411765749":7,*/ /* Scarlet Fireball */
      /*"2411765647":7,*/ /* Golden Fireball  */
      "300000000":7,
      "400000000":7,
         /* rare */
      /*"2411765359":4,*/ /* Midnight Fireball  */
      /*"2411764715":4,*/ /* Blau Fireball  */
      "500000000":4,
      "200000000":4,
   
         /*uncommon */
      /*"2411764596":2,*/ /* Void Fireball */
     /* "2411764415":2,*/ /* Fel Fireball  */

     "600000000":2,
     "100000000":2,
  },
    

}



function AddBoostOld(providedinventory) {

    var boostNumbers = {
        "cybi":0,
        "tertius":0,
        "coyote":0,
        "hammer":0,
        "ram":0,
        "helio":0,
        "trig":0,
        "invin":0,
        "hawk":0,
        "pixie":0,
        "shockray":0,
        "cyclops":0,
        "jax":0,
        "bff":0,
        "striker":0,
        "blade":0,
        "hummingbird":0,
        "spacerock":0
    }

  
    //make sure to remove the "_id":"boost" entry as a precaution
    
    var newinv = [];

    var anyboost  = false;

    for (var skin of providedinventory) {
        //console.log(skin);
        //{ _id: 'cybi_786130582', quantity: 1 }

        if(skin._id == "boost"){
            //skip extra boost field if present
            continue;
        }

        newinv.push(skin);

        //other skins expect to have separator _
        var skinbase = skin._id.split('_')[0];

        if(BOOSTS[skinbase]){
            //console.log(skinbase);
            var skinid = skin._id.split('_')[1];

            if(BOOSTS[skinbase][skinid]){

                var boostval = BOOSTS[skinbase][skinid];
                //console.log(skinbase+" -> "+boostval);

                //check in the output array if it's 
       
                //console.log(boostNumbers[skinbase]);
                //var bigger = Math.max(boostNumbers[skinbase],boostval);
                boostNumbers[skinbase] = Math.max(boostNumbers[skinbase],boostval);
                //console.log(bigger);
                // boostNumbers[skinbase] = Math.max([boostNumbers[skinbase],boostval]);

                anyboost = true;
                
            }


        }

    }

    //lil exit scenario
    if(!anyboost){
        return newinv;
    }

   // console.log(boostNumbers);

    var extraboost = Math.max(boostNumbers["blade"],boostNumbers["hummingbird"]) + boostNumbers["spacerock"];
    //console.log("extra boost = "+extraboost);


    var addedboost = {
            "_id":"boost",
    };

    var b = boostNumbers.cybi + extraboost;
    if(b > 0){
        addedboost["cybi"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.tertius + extraboost;
    if(b > 0){
        addedboost["tertius"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.coyote + extraboost;
    if(b > 0){
        addedboost["coyote"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.hammer + extraboost;
    if(b > 0){
        addedboost["hammer"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.ram + extraboost;
    if(b > 0){
        addedboost["ram"] = {"hp":b, "dmg":b, "deathDmg":b};
    }
    b = boostNumbers.helio + extraboost;
    if(b > 0){
        addedboost["helio"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.trig + extraboost;
    if(b > 0){
        addedboost["trig"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.invin + extraboost;
    if(b > 0){
        addedboost["invin"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.hawk + extraboost;
    if(b > 0){
        addedboost["hawk"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.pixie + extraboost;
    if(b > 0){
        addedboost["pixie"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.shockray + extraboost;
    if(b > 0){
        addedboost["shockray"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.cyclops + extraboost;
    if(b > 0){
        addedboost["cyclops"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.jax + extraboost;
    if(b > 0){
        addedboost["jax"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.bff + extraboost;
    if(b > 0){
        addedboost["bff"] = {"hp":b, "dmg":b};
    }
    b = boostNumbers.striker + extraboost;
    if(b > 0){
        addedboost["striker"] = {"hp":b, "dmg":b};
    }

   //var addedboost = {
   //   "_id":"boost",
   //     "cybi":{"hp":0,"dmg":0},
   //"tertius":{"hp":0, "dmg":0},
   //  "coyote":{"hp":0,"dmg":0},
   //    "hammer":{"hp":0,"dmg":0},
   //      "ram":{"hp":0,"dmg":0,"deathDmg":0},
   //        "helio":{"hp":0,"dmg":0},
   //"trig":{"hp":0,"dmg":0},
   //  "invin":{"hp":0,"dmg":0},
   //    "hawk":{"hp":0,"dmg":0},
   //      "pixie":{"hp":0,"dmg":0},
   //        "shockray":{"hp":0,"dmg":0},
   //"cyclops":{"hp":0,"dmg":0},
   //  "jax":{"hp":0,"dmg":0},
   //    "bff":{"hp":0,"dmg":0},
   //      "striker":{"hp":0,"dmg":0},
   //    };


    newinv.push(addedboost);
  
    return newinv;
  }


  

//adds boost respecting the stacking, if stacking flag is set to true
//else it behaves as old apply boost, (max boost determined by highest rarity skin)
function AddBoost(providedinventory) {

   //set to false to prevent stacking boost from multiples of same rarity nfts per champ
   var applystackingboost = true;

   var boostNumbers = {
       "cybi":0,
       "tertius":0,
       "coyote":0,
       "hammer":0,
       "ram":0,
       "helio":0,
       "trig":0,
       "invin":0,
       "hawk":0,
       "pixie":0,
       "shockray":0,
       "cyclops":0,
       "jax":0,
       "bff":0,
       "striker":0,
       "blade":0,
       "hummingbird":0,
       "spacerock":0,
       "fireball":0
   }

   //so i dont have to rewrite everything, this will keep counter of how many of same "rarity/boost" of each unit we have
   //e.g. if we have 7 rare cybi skins total (doesn't have ot be same asa id, jsut same rarity)

   var boostStacks = {
      "cybi":0,
      "tertius":0,
      "coyote":0,
      "hammer":0,
      "ram":0,
      "helio":0,
      "trig":0,
      "invin":0,
      "hawk":0,
      "pixie":0,
      "shockray":0,
      "cyclops":0,
      "jax":0,
      "bff":0,
      "striker":0,
      "blade":0,
      "hummingbird":0,
      "spacerock":0,
      "fireball":0
   }

 
   //make sure to remove the "_id":"boost" entry as a precaution
   
   var newinv = [];

   var anyboost  = false;

   for (var skin of providedinventory) {
       //console.log(skin);
       //{ _id: 'cybi_786130582', quantity: 1 }

       if(skin._id == "boost"){
           //skip extra boost field if present
           continue;
       }

       newinv.push(skin);

       //other skins expect to have separator _
       var skinbase = skin._id.split('_')[0];

       if(BOOSTS[skinbase]){
           //console.log(skinbase);
          
           var skinid = skin._id.split('_')[1];


           if(BOOSTS[skinbase][skinid]){

               var boostval = BOOSTS[skinbase][skinid];
               //console.log(skinbase+" -> "+boostval);

               //check in the output array if it's 
      
               //console.log(boostNumbers[skinbase]);
               //var bigger = Math.max(boostNumbers[skinbase],boostval);

               //keep track of stacks of highest rarity skin
               if(boostval > boostNumbers[skinbase]){
                  boostStacks[skinbase] = skin.quantity; //we reset the stacking, as we assigned new higher rarity boost
               }else if(boostval == boostNumbers[skinbase]){
                  boostStacks[skinbase] += skin.quantity;//increment the total stack of same rarity
               }

               boostNumbers[skinbase] = Math.max(boostNumbers[skinbase],boostval);
               //console.log(bigger);
               // boostNumbers[skinbase] = Math.max([boostNumbers[skinbase],boostval]);


               anyboost = true;
               
           }


       }

   }

   //lil exit scenario
   if(!anyboost){
       return newinv;
   }



   //apply stacking boost
   //we don't stack ships for now
   if(applystackingboost){
      for (var unit in boostStacks) {

         //skip ships and rocks for now
         if(unit == "blade"){
            continue;
         }else if(unit == "hummingbird"){
            continue;
         }else if(unit == "spacerock"){
            continue;
         }else if(boostStacks[unit] == 1){
            //single skin of this rarity, no stacking
            continue;
         }else if(!boostNumbers[unit]){
            //no skins for specific unit present at all
            continue;
         }
         //

         var baseboost = boostNumbers[unit];
         var extrastacks = boostStacks[unit] - 1;
         var multiplier = 1.0;
         var cap = 13;

         if(baseboost == 13){
            multiplier = 3.5;
            cap = 20;
         }else if(baseboost == 7){
            multiplier = 1.0;
            cap = 13;
         }else if(baseboost == 4){
            multiplier = 0.5;
            cap = 7;
         }else if(baseboost == 2){
            multiplier = 0.34;
            cap = 4;
         }else if(baseboost == 1){
            multiplier = 0.17;
            cap = 2;
         }

         //get the boost
         var stackedboost = Math.min(cap,baseboost + Math.floor(multiplier*extrastacks));
         //console.log("stackedboost"+unit+":"+stackedboost);

         //override with new stacked boost
         boostNumbers[unit] = stackedboost;

      }

   }

   //console.log(boostNumbers);

   var extraboost = Math.max(boostNumbers["blade"],boostNumbers["hummingbird"]) + boostNumbers["spacerock"];
   //console.log("extra boost = "+extraboost);


   var addedboost = {
           "_id":"boost",
   };

   var b = boostNumbers.cybi + extraboost;
   if(b > 0){
       addedboost["cybi"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.tertius + extraboost;
   if(b > 0){
       addedboost["tertius"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.coyote + extraboost;
   if(b > 0){
       addedboost["coyote"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.hammer + extraboost;
   if(b > 0){
       addedboost["hammer"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.ram + extraboost;
   if(b > 0){
       addedboost["ram"] = {"hp":b, "dmg":b, "deathDmg":b};
   }
   b = boostNumbers.helio + extraboost;
   if(b > 0){
       addedboost["helio"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.trig + extraboost;
   if(b > 0){
       addedboost["trig"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.invin + extraboost;
   if(b > 0){
       addedboost["invin"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.hawk + extraboost;
   if(b > 0){
       addedboost["hawk"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.pixie + extraboost;
   if(b > 0){
       addedboost["pixie"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.shockray + extraboost;
   if(b > 0){
       addedboost["shockray"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.cyclops + extraboost;
   if(b > 0){
       addedboost["cyclops"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.jax + extraboost;
   if(b > 0){
       addedboost["jax"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.bff + extraboost;
   if(b > 0){
       addedboost["bff"] = {"hp":b, "dmg":b};
   }
   b = boostNumbers.striker + extraboost;
   if(b > 0){
       addedboost["striker"] = {"hp":b, "dmg":b};
   }
   
   b = boostNumbers.fireball + extraboost;
   if(b > 0){
      addedboost["fireball"] = {"hp":0, "dmg":b};
   }

  //var addedboost = {
  //   "_id":"boost",
  //     "cybi":{"hp":0,"dmg":0},
  //"tertius":{"hp":0, "dmg":0},
  //  "coyote":{"hp":0,"dmg":0},
  //    "hammer":{"hp":0,"dmg":0},
  //      "ram":{"hp":0,"dmg":0,"deathDmg":0},
  //        "helio":{"hp":0,"dmg":0},
  //"trig":{"hp":0,"dmg":0},
  //  "invin":{"hp":0,"dmg":0},
  //    "hawk":{"hp":0,"dmg":0},
  //      "pixie":{"hp":0,"dmg":0},
  //        "shockray":{"hp":0,"dmg":0},
  //"cyclops":{"hp":0,"dmg":0},
  //  "jax":{"hp":0,"dmg":0},
  //    "bff":{"hp":0,"dmg":0},
  //      "striker":{"hp":0,"dmg":0},
  //    };


   newinv.push(addedboost);
 
   return newinv;
 }





 module.exports = {
   AddBoost,
 }