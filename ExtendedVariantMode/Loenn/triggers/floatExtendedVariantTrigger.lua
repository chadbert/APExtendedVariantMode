local trigger = {}

trigger.name = "ExtendedVariantMode/FloatExtendedVariantTrigger"
trigger.placements = {
    name = "trigger",
    data = {
        variantChange = "Gravity",
        enable = true,
        newValue = 1.0,
        revertOnLeave = false,
        revertOnDeath = true,
        delayRevertOnDeath = false,
        withTeleport = false,
        coversScreen = false,
        flag = "",
        flagInverted = false,
        onlyOnce = false
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
    }
}

return trigger
