using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tor_tools
{
    class NodeFileSource
    {
        public Dictionary<string, List<NodeFileSourceItem>> sources = new Dictionary<string, List<NodeFileSourceItem>>();

        public NodeFileSource()
        {
            List<NodeFileSourceItem> item = new List<NodeFileSourceItem>();

            //Tuple<string, string>[] strItems;

            List<NodeFileSourceItem> strItems = new List<NodeFileSourceItem>();

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("ablIconSpec", "icon") };
            sources.Add("abl.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("achIcon", "icon") };            
            sources.Add("ach.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("achCategoriesIcon", "icon"), new NodeFileSourceItem("achCategoriesCodexIcon", "icon") };            
            sources.Add("achCategoriesTable_Prototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("4611686300283124000", "cnv") };            
            sources.Add("arbSkirmishTable_Prototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("4611686299377594005", "fx"), new NodeFileSourceItem("4611686299377594006", "fx") };            
            sources.Add("BlasterVisual_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("bom_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("capacitor_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("cdxImage", "codex") };            
            sources.Add("cdx.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("chrBgDataIcon", "icon") };            
            sources.Add("chrBackgroundTablePrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("chrCompanionInfo_portrait", "gfximg") };            
            sources.Add("chrCompanionInfo_Prototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("chrSpecString", "spec") };            
            sources.Add("chrSpecPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("colCollectionCategoryIcon", "icon") };            
            sources.Add("colCollectionCategoriesPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("colCollectionIcon", "icon") };            
            sources.Add("colCollectionItemsPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fxgr2") };            
            sources.Add("drone_turret_shields", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("dth.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "string") };            
            sources.Add("dyn.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("eng_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("effAppParam_CasterAnim", "anim"), new NodeFileSourceItem("effAppParam_TargetAnim", "anim"), new NodeFileSourceItem("effAppParam_fxSpec", "fx") };
            sources.Add("epp.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("FFSpaceDustMap", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("fxUsedInScriptProto", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("string", "icon") };            
            sources.Add("guiIconsTablePrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("gun_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("string", "string") };            
            sources.Add("hyd.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("hydAnimationAction", "anim") };            
            sources.Add("hydAnimationInfoPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fxgr2") };            
            sources.Add("imp_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("inf_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("itmIcon", "icon") };            
            sources.Add("itm.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("itmModel", "gr2"), new NodeFileSourceItem("itmFxSpec", "fx") };            
            sources.Add("itmAppearanceDatatable", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("itmEnhancementTypeIconAsset", "icon") };            
            sources.Add("itmEnhancementInfoPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("4611687146302020013", "icon") };            
            sources.Add("lgcPerkPrototypeMap", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("4611686334119970107", "icon") };            
            sources.Add("lgcUnlockPrototypeMap", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("ldgScreenName", "load"), new NodeFileSourceItem("ldgOverlayName", "load") };            
            sources.Add("loadingAreaLoadScreenPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("tipIcon", "tip") };            
            sources.Add("loadingScreenTips", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("magazine_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("mbnBinocularsDatatablePrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("mntDataVFX", "gr2") };            
            sources.Add("mntMountInfoPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("mtxStorefrontIcon", "icon") };            
            sources.Add("mtxStorefrontInfoPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("plcModel", "gr2") };            
            sources.Add("plc.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fxgr2") };            
            sources.Add("premium_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("pweap_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("qstMissionIcon", "icon") };            
            sources.Add("qst.", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("reactor_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fxgr2") };            
            sources.Add("rep_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("4611686297701794045", "icon"), new NodeFileSourceItem("4611686297701794046", "icon") };            
            sources.Add("repGroupInfosPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("scFFColorOptionMasterPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fxgr2") };            
            sources.Add("scFFComponentAppearanceDataPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFCrewIcon", "icon"), new NodeFileSourceItem("4611686300849694002", "anim") };            
            sources.Add("scffCrewPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "gr2") };            
            sources.Add("scFFDronesDataPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFPatternIcon", "icon") };            
            sources.Add("scFFPatternsDefinitionProtoype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "dds") };            
            sources.Add("scFFPatternsTextureDataProtoype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFShipModel", "gr2"), new NodeFileSourceItem("scFFShipHullIcon", "icon"), new NodeFileSourceItem("scFFShipIcon", "icon") };            
            sources.Add("scFFShipsDataPrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("sco_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scEngineFXName", "fx") };            
            sources.Add("scShipProtoData", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("sensor_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("shield_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "bnk") };            
            sources.Add("sndareasoundbanks", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("str_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("sweap_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("sys_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("scFFComponentIcon", "icon") };            
            sources.Add("thruster_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("string", "string") };            
            sources.Add("utlhydraanimations", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("string", "string") };            
            sources.Add("utlposturetransitions", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("string", "string") };            
            sources.Add("utlweapontypeanimations", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("vehAppModel", "gr2"), new NodeFileSourceItem("vehAppIdleAnimation", "anim") };            
            sources.Add("vehAppearanceProtoData", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("/", "fx") };            
            sources.Add("vfxOverridesDatatablePrototype", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("achCategoriesIcon", "icon") };            
            sources.Add("armor_", strItems);

            strItems = new List<NodeFileSourceItem> { new NodeFileSourceItem("hydValue", "string") };
            sources.Add("cnv.", strItems);               
        }
    }

    class NodeFileSourceItem
    {
        public string field;
        public string type;
        public string skip;


        //public NodeFileSourceItem() { }

        public NodeFileSourceItem(string field, string type ) 
        {
            this.field = field;
            this.type = type;           
        }

        /*
        public List<NodeFileSourceItem> getList(string[,] items)
        {
            List<NodeFileSourceItem> sourceItems = new List<NodeFileSourceItem>();
            for (int i = 0; i < items.GetLength(0); i++)
            {                
                NodeFileSourceItem sourceItem;                
                sourceItem = new NodeFileSourceItem(items[i, 0].ToString(), items[i, 1].ToString());
                sourceItems.Add(sourceItem);
            }
            return sourceItems;
        }*/
    }
}
