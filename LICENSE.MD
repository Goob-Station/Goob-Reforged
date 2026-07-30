## License

Each file in this codebase includes a license header that clearly defines its
terms. Most unique files in this repository are licensed under the **Goob Access
Guarantee (GAG) v1.0**, our own license, the full text of which lives in
[`GAG-1.0.txt`](GAG-1.0.txt). Files originating from upstream (the Space Station
14 codebase) are licensed under MIT, and some older files still carry MPL-2.0.
Any dual or multiple licenses are explicitly indicated with two or more license
lines in the file header. Please refer to the headers for the exact licensing
terms.

Most media assets are licensed under [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/)
unless stated otherwise. Assets have their license and the copyright in the
metadata file. [Example](https://github.com/space-wizards/space-station-14/blob/master/Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).

Note that some assets are licensed under the non-commercial
[CC-BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/) or similar
non-commercial licenses and will need to be removed if you wish to use this
project commercially.

### What is GAG?

The Goob Access Guarantee is the Mozilla Public License 2.0 with a network
clause added, in the spirit of the AGPL. It is a modified version of the MPL, it
is not the MPL, and it is not published or endorsed by the Mozilla Foundation.

The short version: it works exactly like MPL, except that if you run a public
server, your players are entitled to your source.

SPDX identifier: `LicenseRef-GAG-1.0`

### GAG for Dummies, Forks, and people looking to port our content

**It is file-level copyleft, same as MPL.** GAG only applies to the specific
files it covers. Unlike AGPL, it does not require the entire project using our
code to also be GAG. You can drop a GAG-covered file into any codebase, open or
closed source, as long as the license header stays with that file. New files you
write that contain none of our code are yours to license however you want.

**If you distribute binaries** of a GAG-covered file, you must make that file's
source available. You do not need to release the source of the rest of your
project.

**If you host a server, you must offer your source to your players.** This is
the part that differs from MPL. If people can connect to your build over a
network, Section 3.6 requires you to:

- publish the source for the GAG-covered files, matching the version actually
  running on that server, not an old snapshot
- provide it at no charge
- put the instructions for getting it somewhere players will actually see them,
  in the same interface they are connecting through

This applies whether or not you changed anything. If you are running stock Goob,
a link to this repository satisfies it. If you have modifications, it needs to
point at your fork. A a link in your server info panel is the expected way to do this.

The file-level scope still applies here. You owe source for the covered files
you modified, not for entirely new content you wrote in new files.

**Relicensing to heavier copyleft.** GAG-licensed files can be relicensed under
AGPLv3 or any later version. Unlike MPL, GPLv2 and LGPLv2.1 are *not* valid
targets, since allowing them would let the network clause be stripped off.

**Porting our content into your project.** Take the file, keep the header, and
the rest of your project stays under whatever license you like. Just be aware
that the network clause travels with the file, so if you host it, you owe your
users source for it.

**Porting content into ours.** MIT and MPL-2.0 files can be brought in as-is
under their own headers. Both are compatible with a GAG-licensed codebase
because everything involved is file-level.
