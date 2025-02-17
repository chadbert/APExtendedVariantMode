﻿using System;

namespace ExtendedVariants.Variants {
    public class AllowThrowingTheoOffscreen : AbstractExtendedVariant {
        public override Type GetVariantType() {
            return typeof(bool);
        }

        public override object GetDefaultVariantValue() {
            return false;
        }

        public override object GetVariantValue() {
            return Settings.AllowThrowingTheoOffscreen;
        }

        public override void SetLegacyVariantValue(int value) {
            Settings.AllowThrowingTheoOffscreen = (value != 0);
        }

        protected override void DoSetVariantValue(object value) {
            Settings.AllowThrowingTheoOffscreen = (bool) value;
        }

        public override void Load() {
            // this setting is used elsewhere
        }

        public override void Unload() {
            // this setting is used elsewhere
        }
    }
}
