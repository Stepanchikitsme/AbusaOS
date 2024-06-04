@echo off
color e
dotnet build
color 9
cls
xcopy Resource\liminewp.bmp bin\Debug\net6.0\ISO\boot /q /y
color a
cls
xorriso -as mkisofs -relaxed-filenames -J -R -l -allow-lowercase -o bin\Debug\net6.0\AbusaOS.iso -b boot/limine-bios-cd.bin -no-emul-boot -boot-load-size 4 -boot-info-table --efi-boot boot/limine-uefi-cd.bin -efi-boot-part --efi-boot-image bin\Debug\net6.0\ISO
cls
color F2
echo Built successfully!
pause