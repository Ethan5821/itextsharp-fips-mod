# iTextSharp 4.1.6.2 – FIPS-Compliant Version with Namespace Refactor

This repository contains a modified version of [iTextSharp 4.1.6](https://github.com/itext/itextsharp), adapted for use in commercial applications requiring **FIPS 140-2** compliance and modern .NET compatibility.

## Modifications

- Replaced non-FIPS compliant encryption modules with FIPS-compliant implementations
- Renamed internal `Org.BouncyCastle` namespace to `iTextSharp.Org.BouncyCastle` to avoid conflicts with external BouncyCastle libraries
- Version updated to **4.1.6.2** to reflect these changes while preserving compatibility with the original iTextSharp 4.1.6

All changes are made in accordance with the **LGPL 2.1** license.

## License

This code is licensed under the [GNU Lesser General Public License v2.1](LICENSE).  
You may freely use, modify, and redistribute this modified version under the terms of the LGPL 2.1.

## Disclaimer

This repository contains modified code originally from iTextSharp 4.1.6.  
It is **not affiliated with iText Software** or the maintainers of the official iTextSharp project.
