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
-- Table structure for table `achcategories`
--

DROP TABLE IF EXISTS `achcategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achcategories` (
  `current_version` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `previous_version` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `CatId` bigint(20) unsigned NOT NULL,
  `Name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `NameId` bigint(20) NOT NULL,
  `Index` int(11) NOT NULL,
  `ParentCatId` bigint(20) unsigned NOT NULL,
  `CodexIcon` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Icon` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `SubCategories` varchar(600) COLLATE utf8_unicode_ci NOT NULL,
  `Rows` varchar(3000) COLLATE utf8_unicode_ci NOT NULL,
  `Hash` int(11) NOT NULL,
  PRIMARY KEY (`CatId`),
  UNIQUE KEY `id_UNIQUE` (`CatId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `achcategories`
--

LOCK TABLES `achcategories` WRITE;
/*!40000 ALTER TABLE `achcategories` DISABLE KEYS */;


/*!40000 ALTER TABLE `achcategories` ENABLE KEYS */;
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
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `achcategories_update`
BEFORE UPDATE ON `achcategories`
FOR EACH ROW
BEGIN
  IF ((NOT EXISTS(SELECT 1 FROM achcategories_old_versions WHERE CatId =  OLD.CatId AND Hash = OLD.Hash)) AND (NOT (OLD.Hash = NEW.Hash))) THEN
	INSERT INTO `achcategories_old_versions` (version, CatId, Name, NameId, Index, ParentCatId, CodexIcon, Icon, SubCategories, Rows, Hash)
	VALUES (OLD.current_version, OLD.CatId, OLD.Name, OLD.NameId, OLD.Index, OLD.ParentCatId, OLD.CodexIcon, OLD.Icon, OLD.SubCategories, OLD.Rows, OLD.Hash);
  END IF;
END */;;
DELIMITER ;


DROP TABLE IF EXISTS `achcategories_old_versions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achcategories_old_versions` (
  `version` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `CatId` bigint(20) unsigned NOT NULL,
  `Name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `NameId` bigint(20) NOT NULL,
  `Index` int(11) NOT NULL,
  `ParentCatId` bigint(20) unsigned NOT NULL,
  `CodexIcon` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Icon` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `SubCategories` varchar(600) COLLATE utf8_unicode_ci NOT NULL,
  `Rows` varchar(3000) COLLATE utf8_unicode_ci NOT NULL,
  `Hash` int(11) NOT NULL,
  PRIMARY KEY (`CatId`,`version`),
  UNIQUE KEY `id_UNIQUE` (`CatId`,`version`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `achcategories_old_versions`
--

LOCK TABLES `achcategories_old_versions` WRITE;
/*!40000 ALTER TABLE `achcategories_old_versions` DISABLE KEYS */;

/*!40000 ALTER TABLE `achcategories_old_versions` ENABLE KEYS */;
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
