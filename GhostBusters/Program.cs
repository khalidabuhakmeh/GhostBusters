using System;
using System.Threading.Tasks;
using GhostBusters;

Console.WriteLine("⚡️ peeeew! (Slow lazer)");
await Audio.Play("lazer.wav", new PlaybackOptions { Rate = 0.5, Quality = 1 });

Console.WriteLine("👻 Oooooooo!");
await Audio.Play("ghost.wav");

Console.WriteLine("Don't Cross The Streams!!!!");

// all the ghost busters now
await Task.WhenAll(
    Audio.Play("lazer.wav"),
    Audio.Play("lazer.wav", new PlaybackOptions { Rate = 0.4, Quality = .5 }),
    Audio.Play("lazer.wav", new PlaybackOptions { Rate = 0.8, Quality = 1 }),
    Audio.Play("lazer.wav", new PlaybackOptions { Rate = 0.6, Quality = 1 })
);