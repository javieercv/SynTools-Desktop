cmake_minimum_required(VERSION 3.22)
project(SynToolsQtProbe VERSION 0.1 LANGUAGES CXX)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_CXX_EXTENSIONS OFF)

find_package(Qt6 6.10 REQUIRED COMPONENTS Core Gui Quick Qml QuickControls2 Sql Multimedia Test)
qt_standard_project_setup(REQUIRES 6.10)

add_library(SynToolsQtProbeCore STATIC
    include/OperationCoordinator.hpp include/ProcessRunner.hpp include/ProbeDatabase.hpp include/SafeFiles.hpp include/AudioEngine.hpp include/MediaController.hpp
    src/OperationCoordinator.cpp src/ProcessRunner.cpp src/ProbeDatabase.cpp src/SafeFiles.cpp src/AudioEngine.cpp src/MediaController.cpp)
target_include_directories(SynToolsQtProbeCore PUBLIC include)
target_link_libraries(SynToolsQtProbeCore PUBLIC Qt6::Core Qt6::Sql Qt6::Multimedia Qt6::Gui)

qt_add_executable(SynToolsQtProbe src/main.cpp)
set_target_properties(SynToolsQtProbe PROPERTIES
    MACOSX_BUNDLE TRUE
    MACOSX_BUNDLE_GUI_IDENTIFIER "com.syntools.technical.qt"
    MACOSX_BUNDLE_BUNDLE_NAME "SynTools Qt Probe"
    WIN32_EXECUTABLE TRUE)
qt_add_qml_module(SynToolsQtProbe URI SynToolsProbe VERSION 1.0 QML_FILES qml/Main.qml)
target_link_libraries(SynToolsQtProbe PRIVATE SynToolsQtProbeCore Qt6::Quick Qt6::Qml Qt6::QuickControls2)

enable_testing()
qt_add_executable(SynToolsQtProbeTests tests/ProbeTests.cpp)
target_link_libraries(SynToolsQtProbeTests PRIVATE SynToolsQtProbeCore Qt6::Test)
add_test(NAME SynToolsQtProbeTests COMMAND SynToolsQtProbeTests)
