﻿using Microsoft.Speech.Synthesis;
using System.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace VLC_Control
{
    class Synthesizer
    {
        Request hReq;
        SpeechSynthesizer synth;
        SoundPlayer player;
        Queue<KeyValuePair<string, int>> phrases = new Queue<KeyValuePair<string, int>>();
        bool noMore = true;

        public Synthesizer(Request hReq)
        {
            this.hReq = hReq;
            player = new SoundPlayer();

            synth = new SpeechSynthesizer();
            synth.Volume = 100;
            synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, new System.Globalization.CultureInfo("pt-PT"));
            synth.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synth_SpeakCompleted);
        }

        public void Speak(string text, int rate = 1)
        {
            phrases.Enqueue(new KeyValuePair<string, int>(text, rate));
            if (noMore)
                tts();
        }

        private void tts()
        {
            if (phrases.Count > 0)
            {
                noMore = false;
                KeyValuePair<string, int> phrase = phrases.Dequeue();
                string text = phrase.Key;
                int rate = phrase.Value;
                player.Stream = new System.IO.MemoryStream();
                synth.SetOutputToWaveStream(player.Stream);
                synth.Rate = rate;
                synth.SpeakAsync(text);
            }
            else
            {
                noMore = true;
            }
        }

        public void changeGender(VoiceGender vg = VoiceGender.NotSet)
        {
            synth.SelectVoiceByHints(vg, VoiceAge.NotSet, 0, new System.Globalization.CultureInfo("pt-PT"));
        }

        void synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e) // verificar para que serve esta?
        {
            /*if (player.Stream != null)
            {
                int volume = hReq.getVolume();
                if (volume > 20)
                    hReq.setVolume(10);
                player.Stream.Position = 0;
                player.PlaySync();
                hReq.setVolume(volume);
            }*/
            player.Stream.Position = 0;
            player.PlaySync();
            tts();
        }

        public string getGender() {
            return synth.Voice.Gender == VoiceGender.Male ? "male" : "female";
        }

    }
}
