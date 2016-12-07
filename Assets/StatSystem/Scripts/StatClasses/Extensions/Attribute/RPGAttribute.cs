﻿using System.Collections.Generic;
using System;

namespace RPGSystems.StatSystem {
    /// <summary>
    /// RPGStat that inherits from RPGStatModifiable and implements IStatScalable and IStatLinkable.
    /// </summary>
    public class RPGAttribute : RPGStatModifiable, IStatScalable, IStatLinkable {
        /// <summary>
        /// Used By StatLevelValue Property
        /// </summary>
        private int _statLevelValue;

        /// <summary>
        /// Used By StatLinker Value Property
        /// </summary>
        private int _statLinkerValue;

        /// <summary>
        /// List of all stat linkers applied to the stat
        /// </summary>
        private List<RPGStatLinker> _statLinkers;

        /// <summary>
        /// The value gained by the ScaledStat method
        /// </summary>
        public int StatLevelValue {
            get { return _statLevelValue; }
        }

        /// <summary>
        /// the value gained from all applied stat linkers
        /// </summary>
        public int StatLinkerValue {
            get { return _statLinkerValue; }
        }

        /// <summary>
        /// Gets the stat base value with the StatLevelValue and StatLinkerValue added
        /// </summary>
        public override int StatBaseValue {
            get { return base.StatBaseValue + StatLevelValue + StatLinkerValue; }
        }

        /// <summary>
        /// Constructor that takes a stat asset
        /// </summary>
        public RPGAttribute(RPGAttributeAsset asset) : base(asset) {
            _statLinkers = new List<RPGStatLinker>();
        }

        /// <summary>
        /// Overridable method that scales the class based off the passed value
        /// Triggers the stat's Value Change event
        /// </summary>
        public virtual void ScaleStatToLevel(int level) {
            _statLevelValue = level;
            TriggerValueChange();
        }

        /// <summary>
        /// Add a linker to the stat and listen to it's valueChange event
        /// </summary>
        public void AddLinker(RPGStatLinker linker) {
            if (linker != null) {
                _statLinkers.Add(linker);
                if (linker.LinkedStat != null) {
                    linker.LinkedStat.AddValueListener(OnLinkedStatValueChange);
                }
            }
        }

        /// <summary>
        /// Removes a linker from the stat and stops listening to the value change event
        /// </summary>
        /// <param name="linker"></param>
        public void RemoveLinker(RPGStatLinker linker) {
            if (linker != null) {
                _statLinkers.Remove(linker);
                if (linker.LinkedStat != null) {
                    linker.LinkedStat.RemoveValueListener(OnLinkedStatValueChange);
                }
            }
        }


        /// <summary>
        /// Listens to the attached StatLinker's Linked Stat and Updates 
        /// the StatLinkerValue if a stat linker value changes
        /// </summary>
        private void OnLinkedStatValueChange(RPGStat stat) {
            UpdateLinkerValue();
        }

        /// <summary>
        /// Removes all linkers from the stat
        /// </summary>
        public void ClearLinkers() {
            foreach (var linker in _statLinkers) {
                if (linker.LinkedStat != null) {
                    linker.LinkedStat.RemoveValueListener(OnLinkedStatValueChange);
                }
            }
            _statLinkers.Clear();
        }

        /// <summary>
        /// Update the StatLinkerValue based of the currently applied stat linkers
        /// </summary>
        public void UpdateLinkerValue() {
#if DEBUG_STAT_INFO
            Debug.LogFormat("[Stat Category {0}]: Update Linker value", StatCategoryId);
#endif
            // Get the new linker value from connected linkers
            int newLinkerValue = 0;
            foreach (RPGStatLinker link in _statLinkers) {
                newLinkerValue += link.GetValue();
            }

            // Trigger value change if new value
            if (_statLevelValue != newLinkerValue) {
                _statLevelValue = newLinkerValue;
                TriggerValueChange();
            }
        }
    }
}
