using Doozy.Engine.UI.Input;
using Framework;
using Fxb.SpawnPool;
using UnityEngine;
using VRTKExtensions;
using Fxb.CsvConfig;
using Fxb.Localization;
using Fxb.DA;

namespace Fxb.CMSVR
{
    public class CmsVRWorld : World
    {
        protected override void InitConfig()
        {
            base.InitConfig();

            Config.TARGET_FRAMERATE = 90;

            RegistConfigs<WrenchConfig, WrenchConfig.Item>(PathConfig.CONFIG_WRENCH);

            RegistConfigs<RecordCsvConfig, RecordCsvConfig.Item>(PathConfig.CONFIG_RECORD);

            RegistConfigs<RecordErrorCsvConfig, RecordErrorCsvConfig.Item>(PathConfig.CONFIG_RECORDERROR);

            RegistConfigs<DACsvConfig, DACsvConfig.Item>(PathConfig.CONFIG_DA);

            RegistConfigs<PropCsvConfig, PropCsvConfig.Item>(PathConfig.CONFIG_PROP);

            RegistConfigs<TaskCsvConfig, TaskCsvConfig.Item>(PathConfig.CONFIG_TASK);

            RegistConfigs<TaskStepGroupCsvConfig, TaskStepGroupCsvConfig.Item>(PathConfig.CONFIG_TASKSTEP);

            RegistConfigs<SoftwareCsvConfig, SoftwareCsvConfig.Item>(PathConfig.CONFIG_SOFTWARE);

            ConfigTester();
        }

        protected void ConfigTester()
        {

        }

        protected override void InitFramework()
        {
            base.InitFramework();

            SpawnPoolMgr.Inst.Init();

            LocalizeMgr.Inst.Init();

            BackButton.Instance.BackButtonInputData = new InputData() { InputMode = InputMode.None };

            PanelSpawnTooltipTrigger.defaultTooltipSpawnKey = PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE;

            SpawnPoolMgr.Inst.AddPool(PanelSpawnTooltipTrigger.defaultTooltipSpawnKey, Resources.Load<GameObject>(PanelSpawnTooltipTrigger.defaultTooltipSpawnKey));

            SpawnPoolMgr.Inst.AddPool(PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_S, Resources.Load<GameObject>(PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_S));

            SpawnPoolMgr.Inst.AddPool(PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_L, Resources.Load<GameObject>(PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_L));

            Injecter.Regist<IRecordModel>(new RecordModel());
        }

        private void RegistConfigs<TConfig, TItem>(string csvPath) where TItem : class, new() where TConfig : DynamicCsvConfig<TItem>
        {
            Injecter.Regist(CsvSerializer.Serialize<TConfig,TItem>(Resources.Load<TextAsset>(csvPath).text, 1));
        }
    }
}