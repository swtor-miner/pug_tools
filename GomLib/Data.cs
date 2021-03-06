﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using GomLib.Tables;

namespace GomLib
{
    /// <summary>
    /// GomLib.Tables.ArmorPerLevel.TableData[WeaponSpec][Models.ItemQuality][ItemLevel][Stat]<br/>
    /// Possible Stats: MaxWeaponDamage, MinWeaponDamage, ForcePowerRating, TechPowerRating
    /// </summary>
    public class Data : IDisposable
    {
        public WeaponPerLevel weaponPerLevel;
        public AbilityDamageTable abilityDamageTable;
        public ArmorPerLevel armorPerLevel;
        public ItemBudget itemBudget;
        public ItemModifierPackageTablePrototype itemModifierPackageTablePrototype;
        public ItemRating itemRating;
        public ModificationNames modificationNames;
        public SchematicVariationsPrototype schematicVariationsPrototype;
        public ShieldPerLevel shieldPerLevel;
        public QuestDifficulty questDifficulty;
        public HuntingRadius huntingRadius;

        [Newtonsoft.Json.JsonIgnore]
        private DataObjectModel _dom;

        private bool disposed = false;

        public Data(DataObjectModel dom)
        {
            _dom = dom;
            weaponPerLevel = new WeaponPerLevel(_dom);
            abilityDamageTable = new AbilityDamageTable(_dom);
            armorPerLevel = new ArmorPerLevel(_dom);
            itemBudget = new ItemBudget(_dom);
            itemModifierPackageTablePrototype = new ItemModifierPackageTablePrototype(_dom);
            itemRating = new ItemRating(_dom);
            modificationNames = new ModificationNames(_dom);
            schematicVariationsPrototype = new SchematicVariationsPrototype(_dom);
            shieldPerLevel = new ShieldPerLevel(_dom);
            questDifficulty = new QuestDifficulty(_dom);
            huntingRadius = new HuntingRadius(_dom);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                if(weaponPerLevel != null) weaponPerLevel.Dispose();
                if (abilityDamageTable != null) abilityDamageTable.Dispose();
                if (armorPerLevel != null) armorPerLevel.Dispose();
                if (itemBudget != null) itemBudget.Dispose();
                if (itemModifierPackageTablePrototype != null) itemModifierPackageTablePrototype.Dispose();
                //itemRating.Dispose();
                //modificationNames.Dispose();
                if (schematicVariationsPrototype != null) schematicVariationsPrototype.Dispose();
                if (shieldPerLevel != null) shieldPerLevel.Dispose();
                if (questDifficulty != null) questDifficulty.Dispose();
                if (huntingRadius != null) huntingRadius.Dispose();
            }
            disposed = true;
        }

        ~Data()
        {
            Dispose(false);
        }

        public void Flush()
        {
            _dom = null;
            weaponPerLevel = null;
            abilityDamageTable = null;
            armorPerLevel = null;
            itemBudget = null;
            itemModifierPackageTablePrototype = null;
            itemRating = null;
            modificationNames = null;
            schematicVariationsPrototype = null;
            shieldPerLevel = null;
            huntingRadius = null;
        }
    }
}
