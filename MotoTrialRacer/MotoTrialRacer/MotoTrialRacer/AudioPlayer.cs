/**
 * Copyright (c) 2011-2014 Microsoft Mobile and/or its subsidiary(-ies).
 * All rights reserved.
 *
 * For the applicable distribution terms see the license text file included in
 * the distribution.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace MotoTrialRacer
{
    /// <summary>
    /// The class for handling all the audio
    /// </summary>
    public class AudioPlayer
    {
        private SoundEffect music;
        private SoundEffect motor;
        private SoundEffect ouch;
        private SoundEffect blast;
        private SoundEffect fanfare;
        private SoundEffectInstance musicInstance;
        private SoundEffectInstance motorInstance;
        private SoundEffectInstance fanfareInstance;

        /// <summary>
        /// Loads all the audio files and creates the needed instances
        /// </summary>
        /// <param name="content"></param>
        public AudioPlayer(ContentManager content)
        {
            music = content.Load<SoundEffect>("Sounds/music");
            motor = content.Load<SoundEffect>("Sounds/motor2");
            ouch = content.Load<SoundEffect>("Sounds/ouch");
            blast = content.Load<SoundEffect>("Sounds/blast");
            fanfare = content.Load<SoundEffect>("Sounds/fanfare");
            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Volume = 0.6f;
            musicInstance.Play();
            motorInstance = motor.CreateInstance();
            motorInstance.IsLooped = true;
            motorInstance.Pitch = -0.5f;
            fanfareInstance = fanfare.CreateInstance();
        }

        /// <summary>
        /// Stops the music
        /// </summary>
        public void StopMusic()
        {
            musicInstance.Stop();
        }

        /// <summary>
        /// Starts the music
        /// </summary>
        public void PlayMusic()
        {
            musicInstance.Play();
        }

        /// <summary>
        /// Stops the motor sound
        /// </summary>
        public void StopMotor()
        {
            motorInstance.Stop();
        }

        /// <summary>
        /// Starts the motor sound
        /// </summary>
        public void PlayMotor()
        {
            motorInstance.Play();
        }

        /// <summary>
        /// Stops the tire blast sound
        /// </summary>
        public void PlayBlast()
        {
            blast.Play();
        }

        /// <summary>
        /// Starts the tire blast sound
        /// </summary>
        public void PlayFanfare()
        {
            fanfareInstance.Play();
        }

        /// <summary>
        /// Stops the fanfare sound
        /// </summary>
        public void StopFanfare()
        {
            fanfareInstance.Stop();
        }

        /// <summary>
        /// Starts the fanfare sound
        /// </summary>
        public void PlayOuch()
        {
            ouch.Play();
        }

        /// <summary>
        /// Changes the pitch of the motor sound
        /// </summary>
        /// <param name="pitch"></param>
        public void SetMotorPitch(float pitch)
        {
            if (pitch > -1)
                if (pitch < 1)
                    motorInstance.Pitch = pitch;
                else
                    motorInstance.Pitch = 1.0f;
            else
                motorInstance.Pitch = -1.0f;
        }
    }
}
