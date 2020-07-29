#! /bin/sh

PROJECT_PATH=$(pwd)
UNITY_BUILD_DIR=$(pwd)/Build

ERROR_CODE=1

echo "Running editor test..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
	-projectPath "$PROJECT_PATH" \
	-buildTarget "Windows" \
	-runEditorTests

	if [ $? = 0 ]; then echo "Editor Tests Passed."
		ERROR_CODE=0
	else echo "One or more Editor tests failed. Exited with $?."
		ERROR_CODE=1
	fi

exit $ERROR_CODE