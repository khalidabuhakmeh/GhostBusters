using System;
using System.Threading;
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

var cancellation = new CancellationTokenSource();

var keyBoardTask = Task.Run(() => {
    Console.WriteLine("Press enter to cancel the birthday song...");
    Console.ReadKey();

    // Cancel the task
    cancellation.Cancel();
});

var happyBirthday = Audio.Play(
    "happy-birthday.wav", 
    new PlaybackOptions(), 
    cancellation.Token
);

await Task.WhenAny(keyBoardTask, happyBirthday);

if (cancellation.IsCancellationRequested) {
    Console.WriteLine("🥳 Someone's a party pooper...");
}