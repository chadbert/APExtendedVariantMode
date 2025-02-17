local enums = require("consts.celeste_enums")

local trigger = {}

trigger.name = "ExtendedVariantMode/FloatExtendedVariantFadeTrigger"
trigger.placements = {
    name = "trigger",
    data = {
        variantChange = "Gravity",
        valueA = 1.0,
        valueB = 1.0,
        positionMode = "LeftToRight",
        revertOnDeath = true
    }
}

trigger.fieldInformation = {
    variantChange = {
        options = {
            "AirFriction",
            "AnxietyEffect",
            "BackgroundBlurLevel",
            "BackgroundBrightness",
            "BadelineLag",
            "BlurLevel",
            "BoostMultiplier",
            "CoyoteTime",
            "DashLength",
            "DashSpeed",
            "DelayBetweenBadelines",
            "ExplodeLaunchSpeed",
            "FallSpeed",
            "ForegroundEffectOpacity",
            "Friction",
            "GameSpeed",
            "GlitchEffect",
            "Gravity",
            "HiccupStrength",
            "HyperdashSpeed",
            "JumpHeight",
            "RegularHiccups",
            "RisingLavaSpeed",
            "RoomLighting",
            "RoomBloom",
            "ScreenShakeIntensity",
            "SnowballDelay",
            "SpeedX",
            "SuperdashSteeringSpeed",
            "SwimmingSpeed",
            "WallBouncingSpeed",
            "ZoomLevel"
        },
        editable = true -- TODO change to false when lists are scrollable in Lönn
    },
    positionMode = {
        options = enums.trigger_position_modes,
        editable = false
    }
}

return trigger
