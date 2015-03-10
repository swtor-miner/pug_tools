CREATE DATABASE  IF NOT EXISTS `tor_dump` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `tor_dump`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: 127.0.0.1    Database: tor_dump
-- ------------------------------------------------------
-- Server version	5.5.32

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `item`
--

DROP TABLE IF EXISTS `item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `item` (
  `current_version` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `previous_version` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `Name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `NodeId` bigint(20) unsigned NOT NULL,
  `NameId` bigint(20) NOT NULL,
  `Fqn` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ItemLevel` int(11) NOT NULL,
  `RequiredLevel` int(11) NOT NULL,
  `AppearanceColor` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ArmorSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Binding` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `CombinedRating` int(11) NOT NULL,
  `CombinedRequiredLevel` int(11) NOT NULL,
  `CombinedStatModifiers` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ConsumedOnUse` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ConversationFqn` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `DamageType` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Description` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `DescriptionId` bigint(20) NOT NULL,
  `DisassembleCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Durability` int(11) NOT NULL,
  `EnhancementCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `EnhancementSlots` text COLLATE utf8_unicode_ci NOT NULL,
  `EnhancementSubCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `EnhancementType` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `EquipAbilityId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `GiftRank` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `GiftType` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Icon` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `IsModdable` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `MaxStack` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ModifierSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `MountSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Quality` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Rating` int(11) NOT NULL,
  `RequiredAlignmentInverted` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredClasses` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredGender` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredProfession` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredProfessionLevel` int(11) NOT NULL,
  `RequiredSocialTier` int(11) NOT NULL,
  `RequiredValorRank` int(11) NOT NULL,
  `RequiresAlignment` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiresSocial` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `SchematicId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ShieldSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Slots` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `StatModifiers` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `SubCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `TreasurePackageId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `TreasurePackageSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `UniqueLimit` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `UseAbilityId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Value` int(11) NOT NULL,
  `VendorStackSize` bigint(20) NOT NULL,
  `WeaponSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `TypeBitSet` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Hash` int(11) NOT NULL,
  `StackCount` int(11) NOT NULL,
  `MaxDurability` int(11) NOT NULL,
  `WeaponAppSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL, 
  `Model` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ImperialVOModulation` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RepublicVOModulation` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`NodeId`),
  UNIQUE KEY `id_UNIQUE` (`NodeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `item`
--

LOCK TABLES `item` WRITE;
/*!40000 ALTER TABLE `item` DISABLE KEYS */;


/*!40000 ALTER TABLE `item` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = latin1 */ ;
/*!50003 SET character_set_results = latin1 */ ;
/*!50003 SET collation_connection  = latin1_swedish_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `item_update`
BEFORE UPDATE ON `item`
FOR EACH ROW
BEGIN
  IF ((NOT EXISTS(SELECT 1 FROM item_old_versions WHERE NodeId =  OLD.NodeId AND Hash = OLD.Hash)) AND (NOT (OLD.Hash = NEW.Hash))) THEN
	INSERT INTO `item_old_versions` (version, Name, NodeId, NameId, Fqn, ItemLevel, RequiredLevel, AppearanceColor, ArmorSpec, Binding, CombinedRating, CombinedRequiredLevel, CombinedStatModifiers, ConsumedOnUse, Conversation, ConversationFqn, DamageType, Description, DescriptionId, DisassembleCategory, Durability, EnhancementCategory, EnhancementSlots, EnhancementSubCategory, EnhancementType, EquipAbilityId, GiftRank, GiftType, Icon, IsModdable, MaxStack, ModifierSpec, MountSpec, Quality, Rating, RequiredAlignmentInverted, RequiredClasses, RequiredGender, RequiredProfession, RequiredProfessionLevel, RequiredSocialTier, RequiredValorRank, RequiresAlignment, RequiresSocial, Schematic, SchematicId, ShieldSpec, Slots, StatModifiers, SubCategory, TreasurePackageId, TreasurePackageSpec, UniqueLimit, UseAbilityId, Value, VendorStackSize, WeaponSpec, TypeBitSet, Hash, StackCount, MaxDurability, WeaponAppSpec, Model, ImperialVOModulation, RepublicVOModulation)
	VALUES (OLD.current_version, OLD.Name, OLD.NodeId, OLD.NameId, OLD.Fqn, OLD.ItemLevel, OLD.RequiredLevel, OLD.AppearanceColor, OLD.ArmorSpec, OLD.Binding, OLD.CombinedRating, OLD.CombinedRequiredLevel, OLD.CombinedStatModifiers, OLD.ConsumedOnUse, OLD.ConversationFqn, OLD.DamageType, OLD.Description, OLD.DescriptionId, OLD.DisassembleCategory, OLD.Durability, OLD.EnhancementCategory, OLD.EnhancementSlots, OLD.EnhancementSubCategory, OLD.EnhancementType, OLD.EquipAbilityId, OLD.GiftRank, OLD.GiftType, OLD.Icon, OLD.IsModdable, OLD.MaxStack, OLD.ModifierSpec, OLD.MountSpec, OLD.Quality, OLD.Rating, OLD.RequiredAlignmentInverted, OLD.RequiredClasses, OLD.RequiredGender, OLD.RequiredProfession, OLD.RequiredProfessionLevel, OLD.RequiredSocialTier, OLD.RequiredValorRank, OLD.RequiresAlignment, OLD.RequiresSocial, OLD.SchematicId, OLD.ShieldSpec, OLD.Slots, OLD.StatModifiers, OLD.SubCategory, OLD.TreasurePackageId, OLD.TreasurePackageSpec, OLD.UniqueLimit, OLD.UseAbilityId, OLD.Value, OLD.VendorStackSize, OLD.WeaponSpec, OLD.TypeBitSet, OLD.Hash, OLD.StackCount, OLD.MaxDurability, OLD.WeaponAppSpec, OLD.Model, OLD.ImperialVOModulation, OLD.RepublicVOModulation);
  END IF;
END */;;
DELIMITER ;


DROP TABLE IF EXISTS `item_old_versions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `item_old_versions` (
  `version` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `unused_version` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `NodeId` bigint(20) unsigned NOT NULL,
  `NameId` bigint(20) NOT NULL,
  `Fqn` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ItemLevel` int(11) NOT NULL,
  `RequiredLevel` int(11) NOT NULL,
  `AppearanceColor` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ArmorSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Binding` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `CombinedRating` int(11) NOT NULL,
  `CombinedRequiredLevel` int(11) NOT NULL,
  `CombinedStatModifiers` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ConsumedOnUse` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ConversationFqn` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `DamageType` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Description` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `DescriptionId` bigint(20) NOT NULL,
  `DisassembleCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Durability` int(11) NOT NULL,
  `EnhancementCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `EnhancementSlots` text COLLATE utf8_unicode_ci NOT NULL,
  `EnhancementSubCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `EnhancementType` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `EquipAbilityId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `GiftRank` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `GiftType` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Icon` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `IsModdable` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `MaxStack` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ModifierSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `MountSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Quality` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Rating` int(11) NOT NULL,
  `RequiredAlignmentInverted` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredClasses` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredGender` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredProfession` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiredProfessionLevel` int(11) NOT NULL,
  `RequiredSocialTier` int(11) NOT NULL,
  `RequiredValorRank` int(11) NOT NULL,
  `RequiresAlignment` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RequiresSocial` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `SchematicId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ShieldSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Slots` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `StatModifiers` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `SubCategory` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `TreasurePackageId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `TreasurePackageSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `UniqueLimit` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `UseAbilityId` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Value` int(11) NOT NULL,
  `VendorStackSize` bigint(20) NOT NULL,
  `WeaponSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `TypeBitSet` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Hash` int(11) NOT NULL,
  `StackCount` int(11) NOT NULL,
  `MaxDurability` int(11) NOT NULL,
  `WeaponAppSpec` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Model` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ImperialVOModulation` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `RepublicVOModulation` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`NodeId`,`version`),
  UNIQUE KEY `id_UNIQUE` (`NodeId`,`Hash`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `item_old_versions`
--

LOCK TABLES `item_old_versions` WRITE;
/*!40000 ALTER TABLE `item_old_versions` DISABLE KEYS */;

/*!40000 ALTER TABLE `item_old_versions` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2014-02-10 20:41:25