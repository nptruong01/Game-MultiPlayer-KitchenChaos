using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GradientSlider : Slider {
    public Gradient gradient;

    private Graphic fill;

#if UNITY_EDITOR
    protected override void Reset() {
        base.Reset();
        fill = null;
    }
#endif

    protected override void Set(float input, bool sendCallback = true) {
        base.Set(input, sendCallback);

        if (fill == null && fillRect != null)
            fill = fillRect.GetComponent<Graphic>();

        if (fill) fill.color = gradient.Evaluate(m_Value);
    }
}