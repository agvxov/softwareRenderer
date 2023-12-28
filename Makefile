.PHONY: run clean

main: Graphics.cs Demo.cs
	mcs *.cs -r:System.Windows.Forms.dll -r:System.Drawing.dll -optimize+ -o:softwareRenderer.exe

run: main
	wine *.exe

clean:
	rm *.exe
