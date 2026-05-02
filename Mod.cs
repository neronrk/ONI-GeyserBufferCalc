using HarmonyLib; // Harmony
using KMod; // UserMod2
using System.Collections.Generic; // List
using System.Linq; // Last

namespace AutomaticGeyserCalculation
{
    // Стандартная загрузка мода
    public class AutomaticGeyserCalculation : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
        }
    }
    
    // ------------------------------------------------------
    // Добавляет расчётные данные в панель информации о гейзере
    // ------------------------------------------------------
    
    [HarmonyPatch(typeof(Geyser))]
    [HarmonyPatch("GetDescriptors")]
    public class GeyserInformationPatch
    {
        // === Заголовок категории: Поток ===
        public static LocString CategoryFlowLabel = "<b>Информация о потоке:</b>";
        public static LocString CategoryFlowTooltip = "Расчётные данные, связанные со скоростью потока этого гейзера";
        
        // === Средний поток при активности ===
        public static LocString ActiveFlowLabel = "Средний поток при активности: {0}";
        public static LocString ActiveFlowTooltip = "Этот гейзер выдаёт в среднем {0} в течение активного (не спящего) периода";
        
        // === Буфер извержения ===
        public static LocString EruptionBufferLabel = "Буфер извержения: {0}";
        public static LocString EruptionBufferTooltip = "Необходимо буферизовать {0} для поддержания постоянного потока {1} в течение активного периода";
        
        // === Общий средний поток ===
        public static LocString TotalFlowLabel = "Общий средний поток: {0}";
        public static LocString TotalFlowTooltip = "Этот гейзер выдаёт в среднем {0}, с учётом времени извержений и периодов покоя";
        
        // === Общий средний поток (не проанализирован) ===
        public static LocString HiddenTotalFlowLabel = "Общий средний поток: (Требуется анализ)";
        public static LocString HiddenTotalFlowTooltip = "Общий средний выходной поток этого гейзера с учётом времени извержений и периодов покоя";
        
        // === Буфер покоя ===
        public static LocString DormancyBufferLabel = "Буфер покоя: {0}";
        public static LocString DormancyBufferTooltip = "Необходимо буферизовать {0} выходного потока для поддержания постоянного потока {1} в течение периода покоя";
        
        // === Буфер покоя (не проанализирован) ===
        public static LocString HiddenDormancyBufferLabel = "Буфер покоя: (Требуется анализ)";
        public static LocString HiddenDormancyBufferTooltip = "Какой объём выходного потока следует буферизовать для поддержания среднего потока в течение периода покоя";
        
        // === НАША ФОРМУЛА: Всего за жизнь ===
        public static LocString TotalMassLabel = "Всего за жизнь: {0}";
        public static LocString TotalMassTooltip = "Общий объем ресурса, который гейзер выдаст за всё время своей жизни";
        
        // === Заголовок категории: Тепло ===
        public static LocString CategoryHeatLabel = "<b>Выработка тепла (выше {0}):</b>";
        public static LocString CategoryHeatTooltip = "Количество тепловой энергии, вырабатываемой в течение данной фазы, относительно целевой температуры {0}";
        
        // === Положительная тепловая мощность — нагрев выше 95°C ===
        public static LocString PeakHeatLabel = "При извержении: {0}";
        public static LocString PeakHeatTooltip = "{0} тепловой энергии вырабатывается во время извержения";
        public static LocString ActiveHeatLabel = "Активный: {0}";
        public static LocString ActiveHeatTooltip = "В среднем {0} тепловой энергии вырабатывается в течение активного (не спящего) периода";
        public static LocString TotalHeatLabel = "Общий средний: {0}";
        public static LocString TotalHeatTooltip = "Всего {0} тепловой энергии вырабатывается этим гейзером в среднем за весь срок службы";
        public static LocString HiddenTotalHeatLabel = "Общий средний: (Требуется анализ)";
        public static LocString HiddenTotalHeatTooltip = "Общий средний выход тепловой энергии, включая период покоя";
        public static LocString HeatMassLabel = "Тепловая масса извержения: {0}";
        public static LocString HeatMassTooltip = "Одно извержение выдаёт в общей сложности {0} тепла";
        
        // === Заголовок категории: Охлаждение ===
        public static LocString CategoryCoolLabel = "<b>Охлаждение (до {0}):</b>";
        public static LocString CategoryCoolTooltip = "Количество охлаждения, которое обеспечивает этот гейзер в течение данной фазы, относительно целевой температуры {0}";
        
        // === Отрицательная тепловая мощность — охлаждение ниже 20°C ===
        public static LocString PeakCoolLabel = "При извержении: {0}";
        public static LocString PeakCoolTooltip = "Этот гейзер обеспечивает {0} охлаждения во время извержения";
        public static LocString ActiveCoolLabel = "Активный: {0}";
        public static LocString ActiveCoolTooltip = "В среднем этот гейзер обеспечивает {0} охлаждения в течение активного (не спящего) периода";
        public static LocString TotalCoolLabel = "Общий средний: {0}";
        public static LocString TotalCoolTooltip = "Всего этот гейзер обеспечивает {0} охлаждения в среднем за весь срок службы";
        public static LocString HiddenTotalCoolLabel = "Общий средний: (Требуется анализ)";
        public static LocString HiddenTotalCoolTooltip = "Общий средний выход охлаждения, включая период покоя";
        public static LocString CoolMassLabel = "Тепловая масса извержения: {0}";
        public static LocString CoolMassTooltip = "Одно извержение выдаёт в общей сложности {0} охлаждения";
        
        // === Расчёт для паровых турбин ===
        public static LocString SteamTurbinePower = "Это количество тепловой энергии могло бы полностью питать {0:N1} паровых турбин при правильном направлении";
        public static LocString SteamTurbineRestricted = "Выход пара в течение этого периода может напрямую питать {0:N1} паровых турбин, ограниченных {1} открытыми вентиляционными отверстиями каждая";
        public static LocString SteamTurbineUnrestricted = "Выход пара в течение этого периода может напрямую питать {0:N1} паровых турбин";
        public static LocString SteamTurbineColdWarning = "Выход этого гейзера холодный и потребует подогрева";
        public static LocString SteamTurbineHotWarning = "Выход этого гейзера горячий, и часть энергии может быть потеряна";
        
        // Добавляем наши дескрипторы после базовых
        public static void Postfix(ref Geyser __instance, ref List<Descriptor> __result)
        {
            // ----------
            // Информация о потоке
            // ----------
            
            // Заголовок категории
            __result.Add(new Descriptor(CategoryFlowLabel, CategoryFlowTooltip));
            
            // Предварительный расчёт данных гейзера
            float flow = __instance.configuration.GetEmitRate();
            float eruptingProportion = __instance.configuration.GetIterationPercent();
            float activeFlow = flow * eruptingProportion;
            float activeProportion = __instance.configuration.GetYearPercent();
            float totalFlow = activeFlow * activeProportion;
            float dormantSeconds = __instance.configuration.GetYearOffDuration();
            
            // Средний поток при активности
            string activeFlowStr = GameUtil.GetFormattedMass(activeFlow, GameUtil.TimeSlice.PerSecond);
            __result.Add(new Descriptor(
                string.Format(ActiveFlowLabel, activeFlowStr),
                string.Format(ActiveFlowTooltip, activeFlowStr)
            ).IncreaseIndent());
            
            // Буфер извержения
            float idleSeconds = __instance.configuration.GetOffDuration();
            float eruptionBuffer = activeFlow * idleSeconds;
            string ebufferStr = GameUtil.GetFormattedMass(eruptionBuffer);
            __result.Add(new Descriptor(
                string.Format(EruptionBufferLabel, ebufferStr),
                string.Format(EruptionBufferTooltip, ebufferStr, activeFlowStr)
            ).IncreaseIndent());
            
            // Общий средний поток и буфер покоя требуют анализа
            Studyable component = __instance.GetComponent<Studyable>();
            bool requiresAnalysis = ((bool)component && !component.Studied);
            if (requiresAnalysis)
            {
                __result.Add(new Descriptor(HiddenTotalFlowLabel, HiddenTotalFlowTooltip).IncreaseIndent());
                __result.Add(new Descriptor(HiddenDormancyBufferLabel, HiddenDormancyBufferTooltip).IncreaseIndent());
            }
            else
            {
                string totalFlowStr = GameUtil.GetFormattedMass(totalFlow, GameUtil.TimeSlice.PerSecond);
                __result.Add(new Descriptor(
                    string.Format(TotalFlowLabel, totalFlowStr),
                    string.Format(TotalFlowTooltip, totalFlowStr)
                ).IncreaseIndent());
                
                float dormantBufferSeconds = __instance.configuration.GetYearOffDuration();
                float dormancyBuffer = totalFlow * dormantBufferSeconds;
                string dbufferStr = GameUtil.GetFormattedMass(dormancyBuffer);
                __result.Add(new Descriptor(
                    string.Format(DormancyBufferLabel, dbufferStr),
                    string.Format(DormancyBufferTooltip, dbufferStr, totalFlowStr)
                ).IncreaseIndent());
            }
            
            // === НАША ФОРМУЛА: Всего за жизнь ===
            // Рассчитываем полное время жизни гейзера (Активность + Покой)
            // Защита от деления на ноль, если гейзер не спит (activeProportion == 1)
            if (activeProportion < 1f)
            {
                float totalLifeSeconds = dormantSeconds / (1f - activeProportion);
                
                // Общий объем = Средний поток * Полное время жизни
                float totalMass = totalFlow * totalLifeSeconds;
                
                string totalMassStr = GameUtil.GetFormattedMass(totalMass);
                __result.Add(new Descriptor(
                    string.Format(TotalMassLabel, totalMassStr),
                    TotalMassTooltip
                ).IncreaseIndent());
            }

            // ------------------
            // Тепловые свойства
            // ------------------
            
            Element element = ElementLoader.FindElementByHash(__instance.configuration.GetElement());
            float SHC = element.specificHeatCapacity;
            float temperature = __instance.configuration.GetTemperature();
            const float heatThreshold = 368.15f; // 95°C
            const float coolThreshold = 293.15f; // 20°C
            string heatTempStr = GameUtil.GetFormattedTemperature(heatThreshold);
            string coolTempStr = GameUtil.GetFormattedTemperature(coolThreshold);
            
            float heat = temperature - heatThreshold;
            if (heat > 0)
            {
                __result.Add(new Descriptor(
                    string.Format(CategoryHeatLabel, heatTempStr),
                    string.Format(CategoryHeatTooltip, heatTempStr)
                ));
                
                float peakHeat = SHC * heat * flow * 1000;
                float activeHeat = peakHeat * eruptingProportion;
                float totalHeat = activeHeat * activeProportion;
                float heatMass = peakHeat * __instance.configuration.GetOnDuration();
                bool isSteam = element.id == SimHashes.Steam;
                string label;
                string tooltip;
                string heatEnergyStr;
                
                heatEnergyStr = GameUtil.GetFormattedHeatEnergyRate(peakHeat);
                label = string.Format(PeakHeatLabel, heatEnergyStr);
                tooltip = string.Format(PeakHeatTooltip, heatEnergyStr);
                tooltip += SteamTurbineFootnote(peakHeat, temperature, flow, isSteam);
                __result.Add(new Descriptor(label, tooltip).IncreaseIndent());
                
                heatEnergyStr = GameUtil.GetFormattedHeatEnergyRate(activeHeat);
                label = string.Format(ActiveHeatLabel, heatEnergyStr);
                tooltip = string.Format(ActiveHeatTooltip, heatEnergyStr);
                tooltip += SteamTurbineFootnote(activeHeat, temperature, activeFlow, isSteam);
                __result.Add(new Descriptor(label, tooltip).IncreaseIndent());
                
                if (requiresAnalysis)
                {
                    __result.Add(new Descriptor(HiddenTotalHeatLabel, HiddenTotalHeatTooltip).IncreaseIndent());
                }
                else
                {
                    heatEnergyStr = GameUtil.GetFormattedHeatEnergyRate(totalHeat);
                    label = string.Format(TotalHeatLabel, heatEnergyStr);
                    tooltip = string.Format(TotalHeatTooltip, heatEnergyStr);
                    tooltip += SteamTurbineFootnote(totalHeat, temperature, totalFlow, isSteam);
                    __result.Add(new Descriptor(label, tooltip).IncreaseIndent());
                }
                
                heatEnergyStr = GameUtil.GetFormattedHeatEnergy(heatMass);
                label = string.Format(HeatMassLabel, heatEnergyStr);
                tooltip = string.Format(HeatMassTooltip, heatEnergyStr);
                __result.Add(new Descriptor(label, tooltip).IncreaseIndent());
            }
            
            float cool = coolThreshold - temperature;
            if (cool > 0)
            {
                __result.Add(new Descriptor(
                    string.Format(CategoryCoolLabel, coolTempStr),
                    string.Format(CategoryCoolTooltip, coolTempStr)
                ));
                
                float peakCool = SHC * cool * flow * 1000;
                float activeCool = peakCool * eruptingProportion;
                float totalCool = activeCool * activeProportion;
                float coolMass = peakCool * __instance.configuration.GetOnDuration();
                
                string peakCoolStr = GameUtil.GetFormattedHeatEnergyRate(peakCool);
                __result.Add(new Descriptor(
                    string.Format(PeakCoolLabel, peakCoolStr),
                    string.Format(PeakCoolTooltip, peakCoolStr)
                ).IncreaseIndent());
                
                string activeCoolStr = GameUtil.GetFormattedHeatEnergyRate(activeCool);
                __result.Add(new Descriptor(
                    string.Format(ActiveCoolLabel, activeCoolStr),
                    string.Format(ActiveCoolTooltip, activeCoolStr)
                ).IncreaseIndent());
                
                if (requiresAnalysis)
                {
                    __result.Add(new Descriptor(HiddenTotalCoolLabel, HiddenTotalCoolTooltip).IncreaseIndent());
                }
                else
                {
                    string totalCoolStr = GameUtil.GetFormattedHeatEnergyRate(totalCool);
                    __result.Add(new Descriptor(
                        string.Format(TotalCoolLabel, totalCoolStr),
                        string.Format(TotalCoolTooltip, totalCoolStr)
                    ).IncreaseIndent());
                }
                
                string coolMassStr = GameUtil.GetFormattedHeatEnergy(coolMass);
                __result.Add(new Descriptor(
                    string.Format(CoolMassLabel, coolMassStr),
                    string.Format(CoolMassTooltip, coolMassStr)
                ).IncreaseIndent());
            }
        }
        
        public static string SteamTurbineFootnote(float heatEnergy, float temperature, float flow, bool isSteam = false)
        {
            float turbines;
            string result = "\n\n";
            const float minTemp = 398.15f;
            const float maxTemp = 630.65f;
            if (!isSteam)
            {
                const float maxEfficiency = 877590f;
                turbines = heatEnergy / maxEfficiency;
                result += string.Format(SteamTurbinePower, turbines);
                if (temperature < minTemp)
                {
                    result += "\n\n";
                    result += SteamTurbineColdWarning;
                }
                return result;
            }
            float turbineFlow = 2.0f;
            int vents = 5;
            if (temperature <= 473.15f) { vents = 5; }
            else if (temperature <= 499.4f) { vents = 4; turbineFlow = 1.6f; }
            else if (temperature <= 543.15f) { vents = 3; turbineFlow = 1.2f; }
            else if (temperature <= 630.65f) { vents = 2; turbineFlow = 0.8f; }
            else { vents = 2; turbineFlow = 0.8f; }
            turbines = flow / turbineFlow;
            if (vents == 5)
            {
                result += string.Format(SteamTurbineUnrestricted, turbines);
            }
            else
            {
                result += string.Format(SteamTurbineRestricted, turbines, vents);
            }
            if (temperature < minTemp)
            {
                result += "\n\n";
                result += SteamTurbineColdWarning;
            }
            if (temperature > maxTemp)
            {
                result += "\n\n";
                result += SteamTurbineHotWarning;
            }
            return result;
        }
    }
}