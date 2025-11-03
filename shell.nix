with import <nixpkgs> {};
pkgs.mkShell rec {

  dotnetPkg =
    (with dotnetCorePackages; combinePackages [
      sdk_9_0
    ]);

  deps = [
    zlib
    zlib.dev
    openssl
    dotnetPkg
    fontconfig
  ];

    packages = [
      git
    ];

    #miserable hack
    LD_LIBRARY_PATH = with pkgs; lib.makeLibraryPath ([
      stdenv.cc.cc
      fontconfig
      libxrandr
      xorg.libICE
      xorg.libX11
      xorg.libSM
    ]++ deps);

  NIX_LD_LIBRARY_PATH = lib.makeLibraryPath ([
    stdenv.cc.cc
  ] ++ deps);
  NIX_LD = "${pkgs.stdenv.cc.libc_bin}/bin/ld.so";
  nativeBuildInputs = [
  ] ++ deps;

  shellHook = ''
    DOTNET_ROOT="${dotnetPkg}";
    DOTNET_CLI_TELEMETRY_OPTOUT=1;
  '';
}
