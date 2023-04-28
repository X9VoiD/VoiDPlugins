#!/usr/bin/env bash

orig_pwd=$(pwd)
config="Release"
framework="net6.0"
output="./build"
declare -a extra_options
dry_run="false"
plugins=("Binding/ScriptRunner" "Filter/MeL" "Filter/PrecisionControl" "Filter/Reconstructor" "OutputMode/TouchEmu" "OutputMode/VMultiMode" "OutputMode/WindowsInk")
extra_whitelist=("VoiD.dll" "MathNet.Numerics.dll")
generate_zip="true"
generate_sha256="true"
clean_builds="true"
sources="./src"
single_build=""

contains_space() { [[ "$1" =~ " " ]]; return $?; }

wrap_exec() {
    if [ "$dry_run" == "true" ]; then
        eval echo "Would execute: \"$@\""
    else
        eval "$@"
    fi
}

clean_output() {
    if [ -d "$output" ]; then
        wrap_exec rm -rf "$output"
    fi
}

build_plugin() {
    echo "Building $1"
    echo
    local plugin="$1"
    local plugin_output="$output/${plugin##*/}"
    wrap_exec dotnet publish "$sources/$plugin" -c $config -f $framework -o "$plugin_output" "${extra_options[@]}"
    cleanup_plugin "$plugin_output"

    if [ "$generate_zip" = "true" ]; then
        generate_zip "$plugin_output" "$plugin_output.zip"
    fi
}

cleanup_plugin() {
    local plugin="$1"
    local plugin_name="${plugin##*/}"

    for file in "${plugin}"/*; do
        local filename="${file#$plugin/}"

        if [[ "$filename" != "${plugin_name}.dll" && ! "${extra_whitelist[@]}" =~ "$filename" ]]; then
            echo "  removing: $filename"
            wrap_exec rm "$file"
        fi
    done
}

generate_zip() {
    local files="$1"
    local zip_file="$2"
    local zip_name=${zip_file##*/}

    wrap_exec zip -rj "$zip_file" "$files"

    if [ "$generate_sha256" = "true" ]; then
        local abs_output="$(readlink -f $output)"
        cd $(dirname "$zip_file")
        wrap_exec 'sha256sum "$zip_name" >> "$abs_output/zip_hashes.sha256"'
        cd "$orig_pwd"
    fi

    if [ "$clean_builds" = "true" ]; then
        wrap_exec rm -rf "$files"
    fi
}

while [ $# -gt 0 ]; do
    case $1 in
        --single)
            single_build="$2"
            shift
            ;;
        --sources)
            sources="$2"
            shift
            ;;
        -c=*|--config=*)
            config="${1#*=}"
            ;;
        -c|--config)
            config="$2"
            shift
            ;;
        -f=*|--framework=*)
            framework="${1#*=}"
            ;;
        -f|--framework)
            framework="$2"
            shift
            ;;
        -o=*|--output=*)
            output="${1#*=}"
            ;;
        -o|--output)
            output="$2"
            shift
            ;;
        --dry-run)
            dry_run="true"
            ;;
        --no-zip)
            generate_zip="false"
            ;;
        --no-sha256)
            generate_sha256="false"
            ;;
        --no-clean)
            clean_builds="false"
            ;;
        *)
            if contains_space "$1"; then
                extra_options+=("\"$1\"")
            else
                extra_options+=("$1")
            fi
            ;;
    esac
    shift
done

clean_output

if [[ -n "$single_build" ]]; then
    build_plugin "$single_build"
    exit
fi

for plugin in "${plugins[@]}"; do
    build_plugin "$plugin"
    echo
done
