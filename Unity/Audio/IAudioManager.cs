﻿namespace Atom
{
    public interface IAudioManager
    {
        void Init(bool force = false);

        void Play(int audioId);
    }
}