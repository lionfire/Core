using Stride.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Stride3D.Inputs;

public static class PlatformKeyConversion
{

    // REVIEW: These were assigned by looking at docs.  May want to verify.
    // TODO: Finish remaining codes

    /// <summary>
    /// To Windows Virtual-Key code
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <remarks>
    /// References: 
    /// - https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    /// - https://ultralig.ht/api/1_0/_key_codes_8h_source.html
    /// </remarks>
    public static int ToWindowsVirtualKeyCode(this Stride.Input.Keys key)
        => key switch
        {
            Keys.D0 => 0x30,
            Keys.D1 => 0x31,
            Keys.D2 => 0x32,
            Keys.D3 => 0x33,
            Keys.D4 => 0x34,
            Keys.D5 => 0x35,
            Keys.D6 => 0x36,
            Keys.D7 => 0x37,
            Keys.D8 => 0x38,
            Keys.D9 => 0x39,

            Keys.A => 0x41,
            Keys.B => 0x42,
            Keys.C => 0x43,
            Keys.D => 0x44,
            Keys.E => 0x45,
            Keys.F => 0x46,
            Keys.G => 0x47,
            Keys.H => 0x48,
            Keys.I => 0x49,
            Keys.J => 0x4A,
            Keys.K => 0x4B,
            Keys.L => 0x4C,
            Keys.M => 0x4D,
            Keys.N => 0x4E,
            Keys.O => 0x4F,
            Keys.P => 0x50,
            Keys.Q => 0x51,
            Keys.R => 0x52,
            Keys.S => 0x53,
            Keys.T => 0x54,
            Keys.U => 0x55,
            Keys.V => 0x56,
            Keys.W => 0x57,
            Keys.X => 0x58,
            Keys.Y => 0x59,
            Keys.Z => 0x5A,

            Keys.None => 0,
            Keys.Cancel => 0x03, // TOCONFIRM

            Keys.Back => 0x08,
            Keys.Tab => 0x09,
            //Keys.LineFeed => 0x,
            Keys.Clear => 0x0C,
            
            Keys.Enter => 0x0D,
            //Keys.Return => 0x0D, // Dupe: Enter

            //Keys.Pause => 0x  ,
            Keys.CapsLock => 0x14,
            ////Keys.Capital => 0x  , // DUPE
            //Keys.HangulMode => 0x  ,
            ////Keys.KanaMode => 0x  , // DUPE
            //Keys.JunjaMode => 0x  ,
            //Keys.FinalMode => 0x  ,
            //Keys.HanjaMode => 0x  ,
            ////Keys.KanjiMode => 0x  , // DUPE
            Keys.Escape => 0x1B,
            //Keys.ImeConvert => 0x  ,
            //Keys.ImeNonConvert => 0x  ,
            //Keys.ImeAccept => 0x  ,
            //Keys.ImeModeChange => 0x  ,
            Keys.Space => 0x20,
            Keys.PageUp => 0x21,
            ////Keys.Prior => 0x  , // DUPE
            ////Keys.Next => 0x  , // DUPE
            Keys.PageDown => 0x22,
            Keys.End => 0x23,
            Keys.Home => 0x24,

            Keys.Left => 0x25,
            Keys.Up => 0x26,
            Keys.Right => 0x27,
            Keys.Down => 0x28,

            //Keys.Select => 0x  ,
            //Keys.Print => 0x  ,
            //Keys.Execute => 0x  ,
            //Keys.PrintScreen => 0x  ,
            ////Keys.Snapshot => 0x  , // DUPE
            Keys.Insert => 0x2D,
            Keys.Delete => 0x2E,
            Keys.Help => 0x2F,
            Keys.LeftWin => 0x5B,
            Keys.RightWin => 0x5C,
            Keys.Apps => 0x5D,
            Keys.Sleep => 0x5F,

            Keys.NumPad0 => 0x60,
            Keys.NumPad1 => 0x61,
            Keys.NumPad2 => 0x62,
            Keys.NumPad3 => 0x63,
            Keys.NumPad4 => 0x64,
            Keys.NumPad5 => 0x65,
            Keys.NumPad6 => 0x66,
            Keys.NumPad7 => 0x67,
            Keys.NumPad8 => 0x68,
            Keys.NumPad9 => 0x68,
            Keys.Multiply => 0x6A,
            Keys.Add => 0x6B,
            Keys.Separator => 0x6C,
            Keys.Subtract => 0x6D,
            Keys.Decimal => 0x6E,
            //Keys.NumPadDecimal => 0x6E,
            Keys.Divide => 0x6F,
            Keys.F1 => 0x70,
            Keys.F2 => 0x71,
            Keys.F3 => 0x72,
            Keys.F4 => 0x73,
            Keys.F5 => 0x74,
            Keys.F6 => 0x75,
            Keys.F7 => 0x76,
            Keys.F8 => 0x77,
            Keys.F9 => 0x78,
            Keys.F10 => 0x79,
            Keys.F11 => 0x7A,
            Keys.F12 => 0x7B,
            Keys.F13 => 0x7C,
            //Keys.F14 => 0x  ,
            //Keys.F15 => 0x  ,
            //Keys.F16 => 0x  ,
            //Keys.F17 => 0x  ,
            //Keys.F18 => 0x  ,
            //Keys.F19 => 0x  ,
            //Keys.F20 => 0x  ,
            //Keys.F21 => 0x  ,
            //Keys.F22 => 0x  ,
            //Keys.F23 => 0x  ,
            Keys.F24 => 0x87,
            Keys.NumLock => 0x90,
            Keys.Scroll => 0x91,
            Keys.LeftShift => 0xA0,
            Keys.RightShift => 0xA1,
            Keys.LeftCtrl => 0xA2,
            Keys.RightCtrl => 0xA3,
            Keys.LeftAlt => 0xA4, // VK_LMenu Left MENU key  (see also 0x12)
            Keys.RightAlt => 0xA5,  // VK_RMenu Right MENU key  (see also 0x12)
            Keys.BrowserBack => 0xA6,
            Keys.BrowserForward => 0xA7,
            Keys.BrowserRefresh => 0xA8,
            Keys.BrowserStop => 0xA9,
            Keys.BrowserSearch => 0xAA,
            Keys.BrowserFavorites => 0xAB,
            Keys.BrowserHome => 0xAC,
            Keys.VolumeMute => 0xAD,
            Keys.VolumeDown => 0xAE,
            Keys.VolumeUp => 0xAF,
            //Keys.MediaNextTrack => 0x  ,
            //Keys.MediaPreviousTrack => 0x  ,
            //Keys.MediaStop => 0x  ,
            //Keys.MediaPlayPause => 0x  ,
            //Keys.LaunchMail => 0x  ,
            //Keys.SelectMedia => 0x  ,
            //Keys.LaunchApplication1 => 0x  ,
            //Keys.LaunchApplication2 => 0x  ,
            Keys.Oem1 => 0xBA, // ;:
            ////Keys.OemSemicolon => 0x  , // DUPE
            Keys.OemPlus => 0xBB,
            Keys.OemComma => 0xBC,
            Keys.OemMinus => 0xBD,
            Keys.OemPeriod => 0xBE,
            Keys.Oem2 => 0xBF, // /?
                               ////Keys.OemQuestion => 0x  , // DUPE

            Keys.OemTilde => 0xC0,
            //Keys.Oem3 => 0xC0, // Dupe: OemTilde
            
            Keys.OemOpenBrackets => 0xDB, 
            //Keys.Oem4 => 0xDB, // Dupe: OemOpenBrackets

            Keys.OemPipe => 0xDC,
            //Keys.Oem5 => 0xDC, // Dupe: OemPipe

            Keys.Oem6 => 0xDD,
            ////Keys.OemCloseBrackets => 0x  , // DUPE

            Keys.OemQuotes => 0xDE,
            //Keys.Oem7 => 0xDE, // Dupe: OemQuotes

            Keys.Oem8 => 0xDF,

            Keys.OemBackslash => 0xE2,
            //Keys.Oem102 => 0xE2, // Dupe: Keys.OemBackslash

            Keys.Attn => 0xF6,
            Keys.CrSel => 0xF7,
            Keys.ExSel => 0xF8,
            Keys.EraseEof => 0xF9,
            Keys.Play => 0xFA,
            Keys.Zoom => 0xFB,
            Keys.NoName => 0xFC,
            Keys.Pa1 => 0xFD,
            Keys.OemClear => 0xFE,
            Keys.NumPadEnter => 0x0D,
            _ => 0,
        };
}
