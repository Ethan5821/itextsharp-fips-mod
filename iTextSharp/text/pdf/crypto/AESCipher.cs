using System;
using iTextSharp.Org.BouncyCastle.Crypto.Modes;
using iTextSharp.Org.BouncyCastle.Crypto.Paddings;
using iTextSharp.Org.BouncyCastle.Crypto.Engines;
using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Crypto.Parameters;
/*
 * $Id: AESCipher.cs,v 1.4 2007/03/06 12:29:31 psoares33 Exp $
 *
 * Copyright 2006 Paulo Soares
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */
namespace iTextSharp.text.pdf.crypto {

/**
 * Creates an AES Cipher with CBC and padding PKCS5/7.
 * @author Paulo Soares (psoares@consiste.pt)
 */
    public class AESCipher {
        private PaddedBufferedBlockCipher bp;
        
        /** Creates a new instance of AESCipher */
        public AESCipher(bool forEncryption, byte[] key, byte[] iv) {
            IBlockCipher aes = new AesFastEngine();
            IBlockCipher cbc = new CbcBlockCipher(aes);
            bp = new PaddedBufferedBlockCipher(cbc);
            KeyParameter kp = new KeyParameter(key);
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            bp.Init(forEncryption, piv);
        }
        
        public byte[] Update(byte[] inp, int inpOff, int inpLen) {
            int neededLen = bp.GetUpdateOutputSize(inpLen);
            byte[] outp = null;
            if (neededLen > 0)
                outp = new byte[neededLen];
            else
                neededLen = 0;
            bp.ProcessBytes(inp, inpOff, inpLen, outp, 0);
            return outp;
        }
        
        public byte[] DoFinal() {
            int neededLen = bp.GetOutputSize(0);
            byte[] outp = new byte[neededLen];
            int n = 0;
            try {
                n = bp.DoFinal(outp, 0);
            }
            catch {
                return outp;
            }
            if (n != outp.Length) {
                byte[] outp2 = new byte[n];
                System.Array.Copy(outp, 0, outp2, 0, n);
                return outp2;
            }
            else
                return outp;
        }
        
    }
}