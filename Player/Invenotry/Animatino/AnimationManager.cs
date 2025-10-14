using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using BaseCharacter;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    /// <summary>
    /// Size of inventory
    /// </summary>
    protected float Timer { get; set; }
    protected float IncriTime { get; set; }
    /// <summary>
    /// Freeze Animations
    /// </summary>
    protected bool freeze = false;
    protected int CurrentFrame { get; set; } = 0;
    protected int EndFrame { get; set; } = 0;
    protected int StartFrame { get; set; }
    protected int LoopRemains { get; set; } = 0;
    /// <summary>
    /// Selected item
    /// </summary>
    protected int SelectedItem { get; set; }
    /// <summary>
    /// Currenlty displayed Texture
    /// </summary>
    [SerializeField] Texture display;
    [SerializeField] RawImage rawimage;
    /// <summary>
    /// Animations
    /// </summary>
    protected AnimationSys Anim { get; set; }
    /// <summary>
    /// Set the animation on the UI to the weapon being selected in the Hotbar.
    /// </summary>
    /// <param name="animationClass">Animaiton class</param>
    public void SetAnimation(AnimationSys animationClass)
    {
        LoopRemains = 0;
        try
        {
            Anim = new AnimationSys(animationClass);
            display = Anim.Animation[GetAnimationStart(AnimationType.Idle)];
            rawimage.color = Color.white;
        }
        catch
        {
            display = null;
            rawimage.color = Color.clear;
        }
        
    }
    public AudioClip GetAudioEffect(AnimationType type)
    {
        for (int i = 0; i < Anim.Type.Length; i++)
        {
            if (Anim.Type[i] == type)
            {
                return Anim.Audio[i];
            }
        }
        return null;
    }
    /// <summary>
    /// Get the index in the array to where an animation starts
    /// </summary>
    /// <param name="type">The name of the animation your looking for</param>
    /// <returns>The index via an int</returns>
    public int GetAnimationStart(AnimationType type)
    {
        for (int i = 0; i < Anim.Type.Length; i++)
        {
            if (Anim.Type[i] == type)
            {
                return Anim.CutPoints[i];
            }
        }
        return -1;
    }
    /// <summary>
    /// Get the index in the array to where the animation ends.
    /// </summary>
    /// <param name="type">The name of the animation your looking for.</param>
    /// <returns>the index via an int</returns>
    public int GetAnimationEnd(AnimationType type)
    {
        for (int i = 0; i < Anim.Type.Length; i++)
        {
            if (Anim.Type[i] == type)
            {
                // If this is the last animation type, return the end of the animation
                if (i == Anim.Type.Length - 1)
                {
                    return Anim.Animation.Length;
                }
                // Otherwise return the start of the next animation
                return Anim.CutPoints[i + 1];
            }
        }
        return -1;
    }
    /// <summary>
    /// Setup how fast the frames of an animtion go by for <see cref="SetShowingAnimation(int, int, float)"/>
    /// </summary>
    /// <param name="time">How long</param>
    /// <param name="type">What animation</param>
    /// <returns>A float</returns>
    public float GetAnimTime(float time, AnimationType type)
    {
       return (time) / (GetAnimationEnd(type) - GetAnimationStart(type));
    }
    /// <summary>
    /// Show animations from <c>start</c> to <c>end</c>. Switches images every <c><paramref name="time"/> <br></br>
    /// </summary>
    /// <param name="start">Animation start</param>
    /// <param name="end">Animation end</param>
    /// <param name="time">Automatic timing system example: <code>(0.1f + AtackTime) / (1 + GetAnimationEnd(AnimationType.Shoot) - GetAnimationStart(AnimationType.Shoot))</code></param>
    public void SetShowingAnimation(int start, int end, float time)
    {
        IncriTime = time;
        freeze = false;
        StartFrame = start;
        CurrentFrame = start;
        EndFrame = end;
        FreezeAnimations(false);
    }
    /// <summary>
    /// Show animations from <c>start</c> to <c>end</c>. Switches images every <c><paramref name="time"/> /<paramref name="loop"/></c> <br></br>
    /// </summary>
    /// <param name="start">Animation start</param>
    /// <param name="end">Animation end</param>
    /// <param name="time">Automatic timing system example: <code>(0.1f + AtackTime) / (1 + GetAnimationEnd(AnimationType.Shoot) - GetAnimationStart(AnimationType.Shoot))</code></param>

    public void SetShowingAnimation(int start, int end, float time, int loop)
    {
        LoopRemains = loop;
        IncriTime = time/loop;
        freeze = false;
        StartFrame = start;
        CurrentFrame = start;
        EndFrame = end;
        FreezeAnimations(false);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">True = freeze, False = Don't freeze. Also increase the timer</param>
    public void FreezeAnimations(bool type)
    {
        if (type)
        {
            freeze = true;
        }
        else
        {
            freeze = false;
            Timer = IncriTime + Time.time;
        }
    }
    public void IdleAnimation(int index)
    {
        freeze = true;
        display = Anim.Animation[index];
    }
    /// <summary>
    /// Set a single image to be still for a "idle" image.
    /// </summary>
    /// <param name="type">Set to a Idle type.</param>
    public void IdleAnimation(AnimationType type)
    {
        freeze = true;
        display = Anim.Animation[GetAnimationStart(type)];
    }
    public Texture GetIdleAnimationTexture(AnimationType type)
    {
        return Anim.Animation[GetAnimationStart(type)];
    }
    public void SetAnimationIndex(int index)
    {
        CurrentFrame = index;
    }
    public void ReSetAnimationIndex()
    {
        CurrentFrame = 0;
    }
    /// <summary>
    /// How many loops remain
    /// </summary>
    /// <returns><see cref="LoopRemains"/></returns>
    public int GetLoop()
    {
        return LoopRemains;
    }
    /// <summary>
    /// Reset any looping animations.
    /// </summary>
    public void ReSetLoop()
    {
        LoopRemains = 0;
    }
    /// <summary>
    /// Set the amount of loops.
    /// </summary>
    /// <param name="amount"></param>
    public void SetLoop(int amount)
    {
        LoopRemains = amount;
    }
    private void Update()
    {
        if (freeze == false)
        {
            if (Time.time < Timer)
            {
                try
                {
                    display = Anim.Animation[CurrentFrame];
                }
                catch
                {
                    display = Anim.Animation[^1];
                }
                
            }
            else
            {
                Timer += IncriTime;
                CurrentFrame++;
                if (CurrentFrame >= EndFrame && LoopRemains < 1)
                {
                    freeze = true;
                }
                else if (CurrentFrame >= EndFrame && LoopRemains > 0)
                {
                    LoopRemains--;
                    CurrentFrame = StartFrame;
                }
            }
        }
        rawimage.texture = display;
    }
    private void Start()
    {
        rawimage.color = Color.clear;
    }

}
