// swift-tools-version: 6.0

import PackageDescription

let package = Package(
    name: "SynToolsSwiftProbe",
    platforms: [
        .macOS(.v14)
    ],
    products: [
        .executable(
            name: "SynToolsSwiftProbe",
            targets: ["SynToolsSwiftProbe"]
        )
    ],
    dependencies: [
        .package(
            url: "https://github.com/moreSwift/swift-cross-ui",
            revision: "0f3ec3958b79cdc39a1a3e604516b71ab33a9043"
        )
    ],
    targets: [
        .target(
            name: "SwiftProbeCore",
            path: "Sources/SwiftProbeCore"
        ),
        .executableTarget(
            name: "SynToolsSwiftProbe",
            dependencies: [
                "SwiftProbeCore",
                .product(name: "SwiftCrossUI", package: "swift-cross-ui"),
                .product(name: "DefaultBackend", package: "swift-cross-ui")
            ],
            path: "Sources/SynToolsSwiftProbe"
        ),
        .testTarget(
            name: "SwiftProbeCoreTests",
            dependencies: ["SwiftProbeCore"],
            path: "Tests/SwiftProbeCoreTests"
        )
    ]
)
