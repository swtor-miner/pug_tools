<?php
//update and maintain old values import
error_reporting(E_ALL & ~E_NOTICE);
$dropMongo = false;
if(defined('MongoClean')){
    $dropMongo = true;
}
$delJson = false;
if(defined('delJson')){
    $delJson = true;
}
if($argv[1] !== null) {
    chdir($argv[1]);
    $delJson = true;
}

require_once "J:/xampp/vendor/autoload.php";

// Configuration
$dbname = 'torc_db';
//$dbname = 'torc_test';

// Get the collection cursor
//$backup_db = $m->torc_versions;

$genericSorts = [
    //[[ '$**' => "text" ], [ 'name' => "TextIndex" ]],
    ////["LocalizedName" => 1],
    //["LocalizedName.enMale" => 1],
    //["LocalizedName.frMale" => 1],
    //["LocalizedName.deMale" => 1],
];
$genericSorts2 = [
    //[[   "LocalizedName.enMale" => "text",
    //    "LocalizedName.frMale" => "text",
    //    "LocalizedName.frFemale" => "text",
    //    "LocalizedName.deMale" => "text",
    //    "LocalizedName.deFemale" => "text",
    //], [ "name" => "textIndex"]],
    //["LocalizedName" => 1],
    //["LocalizedName.enMale" => 1],
    //["LocalizedName.frMale" => 1],
    //["LocalizedName.deMale" => 1],
];

$genericSorts3 = [
    //[[
    //    "LocalizedName.enMale" => "text",
    //    "LocalizedName.frMale" => "text",
    //    "LocalizedName.frFemale" => "text",
    //    "LocalizedName.deMale" => "text",
    //    "LocalizedName.deFemale" => "text",
    //    "LocalizedDescription.enMale" => "text",
    //    "LocalizedDescription.frMale" => "text",
    //    "LocalizedDescription.frFemale" => "text",
    //    "LocalizedDescription.deMale" => "text",
    //    "LocalizedDescription.deFemale" => "text",
    //], [ "name" => "textIndex"]],
    //["LocalizedName" => 1],
    //["LocalizedName.enMale" => 1],
    //["LocalizedName.frMale" => 1],
    //["LocalizedName.deMale" => 1],
];
$version = [
    ["current_version" => 1],
    ["previous_versions" => 1],
    ["last_seen" => 1],
    ["removed_in" => 1],
    ["hash" => 1],
];

$data = [
    #Ability
    ['ability', 'Abilities',
        //array_merge($genericSorts, [
        //    ['Level' => 1],
        //    [['MinRange' => 1, 'MaxRange' => 1], []],
        //    ['Cooldown' => 1],
        //    ['ChannelingTime' => 1],
        //    ['CastingTime' => 1],
        //    ['IsPassive' => 1],
        //    ['Pushback' => 1],
        //    ['LocalizedCategoryName' => 1],
        //    ["LocalizedCategoryName.enMale" => 1],
        //    ["LocalizedCategoryName.frMale" => 1],
        //    ["LocalizedCategoryName.deMale" => 1],
        //    [["Cooldown" => 1, "LocalizedName.enMale" => 1], null],
        //    [["Cooldown" => 1, "LocalizedName.frMale" => 1], null],
        //    [["Cooldown" => 1, "LocalizedName.deMale" => 1], null],
        //    [["Cooldown" => -1, "LocalizedName.enMale" => 1], null],
        //    [["Cooldown" => -1, "LocalizedName.frMale" => 1], null],
        //    [["Cooldown" => -1, "LocalizedName.deMale" => 1], null],
        //    [["MaxRange" => 1, "LocalizedName.enMale" => 1], null],
        //    [["MaxRange" => 1, "LocalizedName.frMale" => 1], null],
        //    [["MaxRange" => 1, "LocalizedName.deMale" => 1], null],
        //    [["MaxRange" => -1, "LocalizedName.enMale" => 1], null],
        //    [["MaxRange" => -1, "LocalizedName.frMale" => 1], null],
        //    [["MaxRange" => -1, "LocalizedName.deMale" => 1], null],
        //])
    ],
    ['abilitypackage', 'AbilityPackages'],
    #Achievement
    ['achievement', 'Achievements',
        //[
        //[[   "LocalizedName.enMale" => "text",
        //    "LocalizedName.frMale" => "text",
        //    "LocalizedName.frFemale" => "text",
        //    "LocalizedName.deMale" => "text",
        //    "LocalizedName.deFemale" => "text",
        //    "LocalizedDescription.enMale" => "text",
        //    "LocalizedDescription.frMale" => "text",
        //    "LocalizedDescription.frFemale" => "text",
        //    "LocalizedDescription.deMale" => "text",
        //    "LocalizedDescription.deFemale" => "text",
        //    "LocalizedNonSpoilerDesc.enMale" => "text",
        //    "LocalizedNonSpoilerDesc.frMale" => "text",
        //    "LocalizedNonSpoilerDesc.frFemale" => "text",
        //    "LocalizedNonSpoilerDesc.deMale" => "text",
        //    "LocalizedNonSpoilerDesc.deFemale" => "text",
        //    "Fqn" => "text",
        //], [ "name" => "textIndex"]],
        //["LocalizedName" => 1],
        //["LocalizedName.enMale" => 1],
        //["LocalizedName.frMale" => 1],
        //["LocalizedName.deMale" => 1],
        //['Visibility' => 1],
        //['ItemReward' => 1],
        //['MtxReward' => 1],
        //['TitleReward' => 1],
        //['GsfReward' => 1],
        //['CategoryData.Category.Name' => 1],
        //['CategoryData.SubCategory.Name' => 1],
        //['CategoryData.TertiaryCategory.Name' => 1],
    //]
    ],
    #Advanced Class
    ['advclass', 'AdvancedClasses'],
    #Areas
    ['area', 'Areas',
        //array_merge($genericSorts, [
        //])
    ],
    #Classes
    ['classspec', 'Classes',
        //array_merge($genericSorts, [
        //    ["IsPlayerClass" => 1],
        //    ["IsPlayerAdvancedClass" => 1],
        //    ["NpcsWithThisClass" => 1],
        //])
    ],
    #Codex
    ['codex', 'CodexEntries',
        //array_merge($genericSorts, $version, [
        //    ['LocalizedCategoryName' => 1],
        //    ["LocalizedCategoryName.enMale" => 1],
        //    ["LocalizedCategoryName.frMale" => 1],
        //    ["LocalizedCategoryName.deMale" => 1],
        //    ['Faction' => 1],
        //    ['IsPlanet' => 1],
        //    ['IsHidden' => 1],
        //    ['Level' => 1],
        //    ['ClassRestricted' => 1],
        //])
    ],
    #Collections
    ['collection', 'Collections',
    //    array_merge($genericSorts2, [
    //])
    ],
    ['companion', 'Companions'],
    #Conquests
    ['conquest', 'Conquests'],
    #Conversations
    ['conversation', 'Conversations',
        //array_merge($genericSorts2, [
        //])
    ],
    #Decorations
    ['decoration', 'Decorations'],
    #Galactic Starfighter
    ['gsf', 'Ships'],
    #Item
    ['item', 'Items',
        //array_merge($genericSorts3, [
        //    ['Category' => 1],
        //    ['SubCategory' => 1],
        //    ["Quality" => 1],
        //    ["SimpleCombinedStatModifiers.Endurance" => 1],
        //    ["SimpleCombinedStatModifiers.Mastery" => 1],
        //    ["SimpleCombinedStatModifiers.Presence" => 1],
        //    ["SimpleCombinedStatModifiers.Absorption Rating" => 1],
        //    ["SimpleCombinedStatModifiers.Defense Rating" => 1],
        //    ["SimpleCombinedStatModifiers.Power" => 1],
        //    ["SimpleCombinedStatModifiers.Accuracy Rating" => 1],
        //    ["SimpleCombinedStatModifiers.Alacrity Rating" => 1],
        //    ["SimpleCombinedStatModifiers.Shield Rating" => 1],
        //    ["SimpleCombinedStatModifiers.Critical Rating" => 1],
        //    ["SimpleCombinedStatModifiers.Expertise Rating" => 1],
        //    ["GiftRankNum" => 1],
        //    ["RequiredSocialTier" => 1],
        //    ["RequiredValorRank" => 1],
        //    ["RequiredGender" => 1],
        //    ["TypeBitFlags.IsModdable" => 1],
        //    ["TypeBitFlags.IsCrafted" => 1],
        //    ["TypeBitFlags.IsEquipable" => 1],
        //    ["TypeBitFlags.IsRepTrophy" => 1],
        //    ["TypeBitFlags.IsMtxItem" => 1],
        //    ["BindsToSlot" => 1],
        //    ["TypeBitFlags.Unk8" => 1],
        //    ["TypeBitFlags.Unk800" => 1],
        //    ['CombinedRequiredLevel' => 1],
        //    ['CombinedRating' => 1],
        //    [["CombinedRequiredLevel" => 1, "LocalizedName.enMale" => 1], null],
        //    [["CombinedRequiredLevel" => 1, "LocalizedName.frMale" => 1], null],
        //    [["CombinedRequiredLevel" => 1, "LocalizedName.deMale" => 1], null],
        //    [["CombinedRequiredLevel" => -1, "LocalizedName.enMale" => 1], null],
        //    [["CombinedRequiredLevel" => -1, "LocalizedName.frMale" => 1], null],
        //    [["CombinedRequiredLevel" => -1, "LocalizedName.deMale" => 1], null],
        //    [["AuctionCategory.LocalizedName.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["AuctionCategory.LocalizedName.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["AuctionCategory.LocalizedName.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["AuctionCategory.LocalizedName.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["AuctionCategory.LocalizedName.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["AuctionCategory.LocalizedName.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    [["AuctionSubCategory.LocalizedName.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["AuctionSubCategory.LocalizedName.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["AuctionSubCategory.LocalizedName.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["AuctionSubCategory.LocalizedName.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["AuctionSubCategory.LocalizedName.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["AuctionSubCategory.LocalizedName.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    ['EnhancementSlots.ModificationBase62Id' => 1],
        //    ['WeaponAppSpec' => 1],
        //    ['SoundType' => 1],
        //    ['SchematicB62Id' => 1],
        //    ['ReqArtEquipAuth' => 1],
        //])
    ],
    #Missions
    ['mission', 'Quests',
        //array_merge($genericSorts2, [
        //    ['LocalizedCategory' => 1],
        //    ["LocalizedCategory.enMale" => 1],
        //    ["LocalizedCategory.frMale" => 1],
        //    ["LocalizedCategory.deMale" => 1],
        //    [["LocalizedCategory.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedCategory.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedCategory.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedCategory.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedCategory.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedCategory.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    [["IsRepeatable" => 1, "LocalizedName.enMale" => 1], null],
        //    [["IsRepeatable" => 1, "LocalizedName.frMale" => 1], null],
        //    [["IsRepeatable" => 1, "LocalizedName.deMale" => 1], null],
        //    [["IsRepeatable" => -1, "LocalizedName.enMale" => 1], null],
        //    [["IsRepeatable" => -1, "LocalizedName.frMale" => 1], null],
        //    [["IsRepeatable" => -1, "LocalizedName.deMale" => 1], null],
        //    [["RequiredLevel" => 1, "LocalizedName.enMale" => 1], null],
        //    [["RequiredLevel" => 1, "LocalizedName.frMale" => 1], null],
        //    [["RequiredLevel" => 1, "LocalizedName.deMale" => 1], null],
        //    [["RequiredLevel" => -1, "LocalizedName.enMale" => 1], null],
        //    [["RequiredLevel" => -1, "LocalizedName.frMale" => 1], null],
        //    [["RequiredLevel" => -1, "LocalizedName.deMale" => 1], null],
        //    [["XpLevel" => 1, "LocalizedName.enMale" => 1], null],
        //    [["XpLevel" => 1, "LocalizedName.frMale" => 1], null],
        //    [["XpLevel" => 1, "LocalizedName.deMale" => 1], null],
        //    [["XpLevel" => -1, "LocalizedName.enMale" => 1], null],
        //    [["XpLevel" => -1, "LocalizedName.frMale" => 1], null],
        //    [["XpLevel" => -1, "LocalizedName.deMale" => 1], null],
        //    ['RequiredLevel' => 1],
        //    ['XpLevel' => 1],
        //    ['IsRepeatable' => 1],
        //])
    ],
    ['mtx', 'MtxStoreFronts'],
    ['newcompanion', 'NewCompanions',
        //array_merge($genericSorts2, [
        //])
    ],
    ['npc', 'Npcs',
        //array_merge($genericSorts2, [
        //    ["FqnCategory" => 1],
        //    ["FqnSubCategory" => 1],
        //    ["DetFaction.LocalizedName.enMale" => 1],
        //    ["DetFaction.LocalizedName.frMale" => 1],
        //    ["DetFaction.LocalizedName.deMale" => 1],
        //    ["LocalizedToughness.enMale" => 1],
        //    ["LocalizedToughness.frMale" => 1],
        //    ["LocalizedToughness.deMale" => 1],
        //    ["Toughness" => 1],
        //    ["MinLevel" => 1],
        //    ["MaxLevel" => 1],
        //    ["IsVendor" => 1],
        //    [["MinLevel" => 1, "LocalizedName.enMale" => 1], null],
        //    [["MinLevel" => 1, "LocalizedName.frMale" => 1], null],
        //    [["MinLevel" => 1, "LocalizedName.deMale" => 1], null],
        //    [["MinLevel" => -1, "LocalizedName.enMale" => 1], null],
        //    [["MinLevel" => -1, "LocalizedName.frMale" => 1], null],
        //    [["MinLevel" => -1, "LocalizedName.deMale" => 1], null],
        //    [["MaxLevel" => 1, "LocalizedName.enMale" => 1], null],
        //    [["MaxLevel" => 1, "LocalizedName.frMale" => 1], null],
        //    [["MaxLevel" => 1, "LocalizedName.deMale" => 1], null],
        //    [["MaxLevel" => -1, "LocalizedName.enMale" => 1], null],
        //    [["MaxLevel" => -1, "LocalizedName.frMale" => 1], null],
        //    [["MaxLevel" => -1, "LocalizedName.deMale" => 1], null],
        //    [["DetFaction.LocalizedName.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["DetFaction.LocalizedName.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["DetFaction.LocalizedName.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["DetFaction.LocalizedName.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["DetFaction.LocalizedName.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["DetFaction.LocalizedName.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedToughness.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedToughness.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedToughness.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedToughness.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedToughness.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedToughness.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    [["FqnCategory" => 1, "LocalizedName.enMale" => 1], null],
        //    [["FqnCategory" => 1, "LocalizedName.frMale" => 1], null],
        //    [["FqnCategory" => 1, "LocalizedName.deMale" => 1], null],
        //    [["FqnCategory" => -1, "LocalizedName.enMale" => 1], null],
        //    [["FqnCategory" => -1, "LocalizedName.frMale" => 1], null],
        //    [["FqnCategory" => -1, "LocalizedName.deMale" => 1], null],
        //    [["FqnSubCategory" => 1, "LocalizedName.enMale" => 1], null],
        //    [["FqnSubCategory" => 1, "LocalizedName.frMale" => 1], null],
        //    [["FqnSubCategory" => 1, "LocalizedName.deMale" => 1], null],
        //    [["FqnSubCategory" => -1, "LocalizedName.enMale" => 1], null],
        //    [["FqnSubCategory" => -1, "LocalizedName.frMale" => 1], null],
        //    [["FqnSubCategory" => -1, "LocalizedName.deMale" => 1], null],
        //])
    ],
    #Placeables
    ['object', 'Placeables',
        //array_merge($genericSorts, [
        //])
    ],
    ['schematic', 'Schematics',
        //array_merge($genericSorts2, [
        //    ['SkillOrange' => 1],
        //    ['LocalizedCategory' => 1],
        //    ["LocalizedCategory.enMale" => 1],
        //    ["LocalizedCategory.frMale" => 1],
        //    ["LocalizedCategory.deMale" => 1],
        //    ['LocalizedCrewSkillName' => 1],
        //    ["LocalizedCrewSkillName.enMale" => 1],
        //    ["LocalizedCrewSkillName.frMale" => 1],
        //    ["LocalizedCrewSkillName.deMale" => 1],
        //    ['LocalizedSubTypeName' => 1],
        //    ["LocalizedSubTypeName.enMale" => 1],
        //    ["LocalizedSubTypeName.frMale" => 1],
        //    ["LocalizedSubTypeName.deMale" => 1],
        //    [["LocalizedSubTypeName.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedSubTypeName.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedSubTypeName.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedSubTypeName.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedSubTypeName.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedSubTypeName.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedCrewSkillName.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedCrewSkillName.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedCrewSkillName.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedCrewSkillName.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedCrewSkillName.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedCrewSkillName.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedCategory.enMale" => 1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedCategory.frMale" => 1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedCategory.deMale" => 1, "LocalizedName.deMale" => 1], null],
        //    [["LocalizedCategory.enMale" => -1, "LocalizedName.enMale" => 1], null],
        //    [["LocalizedCategory.frMale" => -1, "LocalizedName.frMale" => 1], null],
        //    [["LocalizedCategory.deMale" => -1, "LocalizedName.deMale" => 1], null],
        //    [["SkillOrange" => 1, "LocalizedName.enMale" => 1], null],
        //    [["SkillOrange" => 1, "LocalizedName.frMale" => 1], null],
        //    [["SkillOrange" => 1, "LocalizedName.deMale" => 1], null],
        //    [["SkillOrange" => -1, "LocalizedName.enMale" => 1], null],
        //    [["SkillOrange" => -1, "LocalizedName.frMale" => 1], null],
        //    [["SkillOrange" => -1, "LocalizedName.deMale" => 1], null],
        //])
    ],
    ['setbonus', 'SetBonuses'],
    ['stronghold', 'Strongholds'],
    ['talent', 'Talents'],
];

$skipped = array();
$time_start = microtime(true);

function flattenObject($obj) {
    $ritit = new RecursiveIteratorIterator(new RecursiveArrayIterator($obj));
    $result = array();
    foreach ($ritit as $leafValue) {
        $keys = array();
        foreach (range(0, $ritit->getDepth()) as $depth) {
            $keys[] = $ritit->getSubIterator($depth)->key();
        }
        $result[ join('.', $keys) ] = $leafValue;
    }
    return $result;
}

function unset_version(&$array) {
    $curstr = 'current_version';
    foreach(array_keys($array) as $arkey){
        if(substr_compare($arkey, $curstr, -strlen($curstr)) === 0) {
            //echo "$arkey\n";
            unset($array[$arkey]);
        }
    }
    unset($array["previous_version"]);
    unset($array["first_seen"]);
    unset($array["last_seen"]);
    unset($array["changed_fields"]);
}

echo "\n";
foreach($data as &$row) {
    if($row[2] !== null)
        $row[2] = array_merge($row[2],  $version);
    $colname = $row[0];
    $c_mongo = (new MongoDB\Client)->$dbname->$colname;
    if($dropMongo){ 
        $c_mongo->drop();
    }
    //$v_mongo = $backup_db->$row[0];

    $c_mongo ->createIndex(array('Base62Id' => 1), array('unique' => true));
    $c_mongo ->createIndex(array('removed_in' => 1));
    //$v_mongo ->createIndex(array('Base62Id' => 1, 'current_version' => 1), array('unique' => true));

    $varId = $row[1];
    if(file_exists("json/Full$varId.json.gz")) {
        $command = escapeshellcmd("gzip -k -d -f json/Full$varId.json.gz");
        exec($command);
    }
    if(file_exists("json/Full$varId.json")) {
            $handle = fopen("json/Full$varId.json", "r");
            $loop_start = microtime(true);
            echo "Starting $varId - ";
        	if($handle) {
                $patch_version = trim(fgets($handle));
                while(($line = fgets($handle)) !== false){
                    $segments = explode(',', $line, 3);
//                    print_r($segments); die();
                    $bId = $segments[0]; //substr($line, 0, 7);
                    $hash = $segments[1];
                    $line = $segments[2]; //substr($line, 8);
                    $decoded = json_decode($line, true);
                    $decoded["hash"] = $hash;
                    $found = $c_mongo->findOne(array('Base62Id' => $bId));
                    $decoded["previous_versions"] = [];
                    $decoded["current_version"] = $patch_version;
                    //$decoded["last_seen"] = $patch_version;
                    if($found == NULL) {
                        //echo "new[$bId] ";
                        $decoded["first_seen"] = $patch_version;
                        $c_mongo->insertOne($decoded);
                    }
                    else {
                        if($found["hash"] == $hash) {
//                            echo "skipped ";
//                            $c_mongo->updateOne(
//                                array("Base62Id" => $bId),
//                                array('$set' => array('last_seen' => $patch_version))
//                            );
                            continue;
                        }
                        $foundflattented = flattenObject($found);
                        $prevVersion = $found['current_version'];
                        $first_seen = $found["first_seen"];
                        $flattened = flattenObject($decoded);
                        $changes = array_diff_assoc($flattened, $foundflattented);
                        unset_version($changes);
                        if(count($changes) > 0){
                            //echo "changed[$bId] ";
                            $decoded["previous_versions"][] = $prevVersion;
                            $decoded["first_seen"] = $first_seen;
                            $decoded["changed_fields"] = array_keys($changes);
                            try {
                                // store the old version in the versioning db
                                //$v_mongo->update(
                                //    ["Base62Id" => $bId, "current_version" => $prevVersion],
                                //    $found,
                                //    ['upsert' => true]
                                //);
                                // overwrite the entry with the new version in the current db
                                $c_mongo->updateOne(
                                    ["Base62Id" => $bId],
                                    $decoded
                                );
                            }
                            catch(Exception $e) {
                              echo $e->getMessage() , "\n";
                            }
                        }
                        else {
                            echo $bId, "shouldn't happen anymore\n"; die();
                            //echo "<pre>"; print_r($decoded);
//                            $c_mongo->updateOne(
//                                array("Base62Id" => $bId),
//                                array('$set' => array('last_seen' => $patch_version))
//                            );
                            //$after = $c_mongo->findOne(array('Base62Id' => $bId));
                            //print_r($after); echo "<pre>"; die();
                            //die();
                        }
                         
                    }
                }
//                if($patch_version !== null) {
//                    $removed = $c_mongo->updateMany(
//                        array('$and' => array(
//                            array('last_seen' => ['$ne' => $patch_version]),
//                            array('removed_in' => ['$exists' => false])
//                        )),
//                        array('$set' => array('removed_in' => $patch_version))
//                    );
//                }
                fclose($handle);
                if(!empty($row[2]))
                    foreach($row[2] as $indexArr) {
                        print_r($indexArr);
                        echo "\n";
                        if(count($indexArr) == 2) {
                            if($indexArr[1] !== null)
                                $c_mongo->createIndex($indexArr[0], $indexArr[1]);
                            else
                                $c_mongo->createIndex($indexArr[0]);
                        }
                        else
                            $c_mongo->createIndex($indexArr);
                    }
                $loop_end = microtime(true);
                $loop_time = round($loop_end - $loop_start, 2);

                echo "Complete in $loop_time seconds\n";
                if ($delJson)
                    unlink("json/Full$varId.json");
            }
            else
                echo "$varId failed\n";
    }
    else
        $skipped[] = $varId;
}
echo "\n-----------------------------------\n";
if(count($skipped))
    echo implode(', ', $skipped)." skipped.\n";
$time_end = microtime(true);
$time = round($time_end - $time_start, 2);
echo "\nCompleted all imports in $time seconds.\n";
?>

