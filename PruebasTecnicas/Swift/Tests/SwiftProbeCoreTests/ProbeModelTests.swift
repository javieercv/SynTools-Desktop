import XCTest
@testable import SwiftProbeCore

final class ProbeModelTests: XCTestCase {
    func testSectionsRemainStable() {
        XCTAssertEqual(
            ProbeSection.allCases.map(\.rawValue),
            ["Inicio", "Herramienta de prueba", "Historial", "Ajustes"]
        )
    }

    func testEverySectionHasDescription() {
        for section in ProbeSection.allCases {
            XCTAssertFalse(section.description.isEmpty)
        }
    }

    func testPlatformDescriptionIsNeverEmpty() {
        XCTAssertFalse(ProbePlatform.displayName.isEmpty)
    }
}
