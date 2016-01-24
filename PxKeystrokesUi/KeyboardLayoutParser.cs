﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesUi
{
    public class KeyboardLayoutParser
    {
        public static string Parse(KeyboardRawEventArgs e)
        {
            StringBuilder sb = new StringBuilder(128);
            int lParam = 0;
            // Bits in lParam
            // 16-23	Scan code.
            // 24	Extended-key flag. Distinguishes some keys on an enhanced keyboard.
            // 25	"Do not care" bit. The application calling this function sets this 
            //      bit to indicate that the function should not distinguish between left 
            //      and right CTRL and SHIFT keys, for example.

            lParam = e.Kbdllhookstruct.scanCode << 16;

            int result = NativeMethodsKeyboard.GetKeyNameText(lParam, sb, 128);
            return sb.ToString();
        }

        public static string ParseViaMapKeycode(KeyboardRawEventArgs e)
        {
            uint r = NativeMethodsKeyboard.MapVirtualKey((uint)e.vkCode, 
                                                    NativeMethodsKeyboard.MAPVK_VK_TO_CHAR);
            return ((char)r).ToString();
        }


        public static string ParseViaToAscii(KeyboardRawEventArgs e)
        {
            byte[] inBuffer = new byte[2];
            int buffertype = NativeMethodsKeyboard.ToAscii(e.vkCode,
                        e.Kbdllhookstruct.scanCode,
                        e.keyState,
                        inBuffer,
                        e.Alt ? 1 : 0);

            if (buffertype < 0) // deadkey
            {

            }
            else if (buffertype == 1) // one char in inBuffer[0]
            {
                char key = (char)inBuffer[0];
                return key.ToString();
            }
            else if (buffertype == 2) // two chars in inBuffer
            {
                char key = (char)inBuffer[0];
                char key2 = (char)inBuffer[1];
                return key.ToString() + key2.ToString();
            }
            else if (buffertype == 0)
            {
                // no translation
            }
            return "";
        }


        public static string ParseViaToUnicode(KeyboardRawEventArgs e)
        {
            StringBuilder inBuffer = new StringBuilder(128);
            int buffertype = NativeMethodsKeyboard.ToUnicode(e.vkCode,
                        e.Kbdllhookstruct.scanCode,
                        e.keyState,
                        inBuffer,
                        128,
                        (uint)(e.Alt ? 1 : 0));

            Log.e("KP",
                    String.Format("   ToUnicode: bl {0} str {1} alt {2} vk {3}", buffertype,
                        inBuffer.ToString(), e.Alt, e.vkCode));

            if (buffertype < 0) // deadkey
            {
                
            }
            else if (buffertype >= 1) // buffertype chars in inBuffer[0..buffertype]
            {
                return inBuffer.ToString(0, buffertype);
            }
            else if (buffertype == 0)
            {
                // no translation
            }
            return "";
        }

        /*
        int convertVirtualKeyToWChar(int virtualKey, PWCHAR outputChar, PWCHAR deadChar)
        {
            int i = 0;
            short state = 0;
            int capsLock;
            int shift = -1;
            int mod = 0;
            int charCount = 0;
            WCHAR baseChar;
            WCHAR diacritic;
            *outputChar = 0;
            capsLock = (GetKeyState(VK_CAPITAL) & 0x1);
            do
            {
                state = GetAsyncKeyState(pgCharModifiers->pVkToBit[i].Vk);
                if (pgCharModifiers->pVkToBit[i].Vk == VK_SHIFT)
                    shift = i + 1; // Get modification number for Shift key
                if (state & ~SHRT_MAX)
                {
                    if (mod == 0)
                        mod = i + 1;
                    else
                        mod = 0; // Two modifiers at the same time!
                }
                i++;
            }
            while (pgCharModifiers->pVkToBit[i].Vk != 0);

            SEARCH_VK_IN_CONVERSION_TABLE(1)
            SEARCH_VK_IN_CONVERSION_TABLE(2)
            SEARCH_VK_IN_CONVERSION_TABLE(3)
            SEARCH_VK_IN_CONVERSION_TABLE(4)
            SEARCH_VK_IN_CONVERSION_TABLE(5)
            SEARCH_VK_IN_CONVERSION_TABLE(6)
            SEARCH_VK_IN_CONVERSION_TABLE(7)
            SEARCH_VK_IN_CONVERSION_TABLE(8)
            SEARCH_VK_IN_CONVERSION_TABLE(9)
            SEARCH_VK_IN_CONVERSION_TABLE(10)

            if (*deadChar != 0) // I see dead characters...
            {
                i = 0;
                do
                {
                    baseChar = (WCHAR) pgDeadKey[i].dwBoth;
                    diacritic = (WCHAR) (pgDeadKey[i].dwBoth >> 16);
                    if ((baseChar == *outputChar) && (diacritic == *deadChar))
                    {
                        *deadChar = 0;
                        *outputChar = (WCHAR) pgDeadKey[i].wchComposed;
                    }
                    i++;
                }
                while (pgDeadKey[i].dwBoth != 0);
            }
            return charCount;
            }*/
      }
}
