using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CliWrap;

namespace GhostBusters
{
    public static class Audio
    {
        public static async Task<PlaybackResult> Play(
            string filename, 
            PlaybackOptions options = null 
        )
        {
            options??= new PlaybackOptions();
            var arguments = options.GetArguments();
            // add filename
            arguments.Add(filename);
            
            var audio = await SimpleExec.Command.ReadAsync(
                "afplay",
                string.Join(" ", arguments),
                noEcho: true
            );

            return new PlaybackResult(audio);
        }

        public static async Task<PlaybackResult> Play(
            string filename,
            PlaybackOptions options,
            CancellationToken token
        )
        {
            options??= new PlaybackOptions();
            var arguments = options.GetArguments();
            // add filename
            arguments.Add(filename);

            var sb = new StringBuilder();
            var command =
                await Cli
                    .Wrap("afplay")
                    .WithArguments(string.Join(" ", arguments))
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb))
                    .ExecuteAsync(token);

            var output = sb.ToString();
            return new PlaybackResult(output);
        }
        
    }
    
    public record PlaybackResult 
    {
        public PlaybackResult(string result)
        {
            if (result == null)
                return;

            var reader = new StringReader(result);
            Filename = reader.ReadLine()?.Split(':')[1].Trim();
            
            var line = reader.ReadLine();
            Format = line?.Substring(line.LastIndexOf(':') + 1).Trim();

            var sizes = reader.ReadLine()?.Split(',');

            if (sizes != null)
            {
                BufferByteSize = int.Parse(sizes[0].Split(':').LastOrDefault() ?? "0");
                NumberOfPacketsToRead = int.Parse(sizes[1].Split(':').LastOrDefault() ?? "0");
            }
        }
        
        public string Filename { get; }
        public string Format { get; }
        public int BufferByteSize { get; }
        public int NumberOfPacketsToRead { get; }
    }

    public class PlaybackOptions
    {
        /// <summary>
        /// Set the volume for playback of the file
        /// Apple does not define a value range for this, but it appears to accept
        /// 0=silent, 1=normal (default) and then up to 255=Very loud.
        /// The scale is logarithmic and in addition to (not a replacement for) other volume control(s).
        /// </summary>
        public int? Volume { get; init; }
        /// <summary>
        /// Play for Time in Seconds
        /// </summary>
        public int? Time { get; init; }
        /// <summary>
        ///  Play at playback RATE.
        ///  practical limits are about 0.4 (slower) to 3.0 (faster).
        /// </summary>
        public double? Rate { get; init; }
        
        /// <summary>
        /// Set the quality used for rate-scaled playback (default is 0 - low quality, 1 - high quality).
        /// </summary>
        public double? Quality { get; init; }

        internal List<string> GetArguments()
        {
            var arguments = new List<string> { "-d" };

            if (Volume.HasValue) {
                arguments.Add($"-v {Volume}");
            }
            if (Time.HasValue)  {
                arguments.Add($"-t {Time}");
            }
            if (Rate.HasValue)  {
                arguments.Add($"-r {Rate}");
            }
            if (Quality.HasValue)  {
                arguments.Add($"-q {Quality}");
            }

            return arguments;
        }
    }
}