# Official uploader workspace template

This directory is a TEMPLATE. It contains metadata, preview, and a development manifest but no compiled DLL. Do not upload this template.

Build.cmd produces dist/CraftTheSpire-beta, with workshop.json and image.png at the root and CraftTheSpire.dll, CraftTheSpire.pck, CraftTheSpire.json in content/.

Use the official Mega Crit uploader: ModUploader.exe upload -w <workspace-directory>.

Keep mod_id.txt after first upload. Never reuse another mod's ID. Workshop dependencies contain numeric item IDs; the game manifest separately declares the STS2-RitsuLib runtime dependency.
