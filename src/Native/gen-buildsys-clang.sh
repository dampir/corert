#!/usr/bin/env bash
#
# This file invokes cmake and generates the build system for Clang.
#

if [ $# -lt 3 -o $# -gt 5 ]
then
  echo "Usage..."
  echo "gen-buildsys-clang.sh <path to top level CMakeLists.txt> <ClangMajorVersion> <ClangMinorVersion> <Architecture> [build flavor]"
  echo "Specify the path to the top level CMake file - <corert>/src/Native"
  echo "Specify the clang version to use, split into major and minor version"
  echo "Specify the target architecture." 
  echo "Optionally specify the build configuration (flavor.) Defaults to DEBUG." 
  exit 1
fi

# Set up the environment to be used for building with clang.
if command -v "clang-$2.$3" > /dev/null 2>&1
    then
        export CC="$(command -v clang-$2.$3)"
        export CXX="$(command -v clang++-$2.$3)"
elif command -v "clang$2$3" > /dev/null 2>&1
    then
        export CC="$(command -v clang$2$3)"
        export CXX="$(command -v clang++$2$3)"
elif command -v clang > /dev/null 2>&1
    then
        export CC="$(command -v clang)"
        export CXX="$(command -v clang++)"
elif command -v gcc > /dev/null 2>&1
    then
        export CC="$(command -v gcc)"
        export CXX="$(command -v g++)"
else
    echo "Unable to find Clang Compiler"
    exit 1
fi

build_arch="$4"
if [ -z "$5" ]; then
    echo "Defaulting to DEBUG build."
    build_type="DEBUG"
else
    # Possible build types are DEBUG, RELEASE
    build_type="$(echo $5 | awk '{print toupper($0)}')"
    if [ "$build_type" != "DEBUG" ] && [ "$build_type" != "RELEASE" ]; then
        echo "Invalid Build type, only debug or release is accepted."
        exit 1
    fi
fi

cmake_extra_defines=
if [[ -n "$LLDB_LIB_DIR" ]]; then
    cmake_extra_defines="$cmake_extra_defines -DWITH_LLDB_LIBS=$LLDB_LIB_DIR"
fi
if [[ -n "$LLDB_INCLUDE_DIR" ]]; then
    cmake_extra_defines="$cmake_extra_defines -DWITH_LLDB_INCLUDES=$LLDB_INCLUDE_DIR"
fi
if [[ -n "$CROSSCOMPILE" ]]; then
    if ! [[ -n "$ROOTFS_DIR" ]]; then
        echo "ROOTFS_DIR not set for crosscompile"
        exit 1
    fi
    cmake_extra_defines="$cmake_extra_defines -C $1/cross/$build_arch/tryrun.cmake"
    cmake_extra_defines="$cmake_extra_defines -DCMAKE_TOOLCHAIN_FILE=$1/cross/$build_arch/toolchain.cmake"
fi

cmake \
  "-DCMAKE_BUILD_TYPE=$build_type" \
  $cmake_extra_defines \
  "$1/src/Native"
