#!/usr/bin/env python3
"""Harness sintético: crea un descendiente y registra PID sin usar datos del usuario."""

import argparse
import os
import pathlib
import subprocess
import sys
import time


def write_pid(path: pathlib.Path, role: str) -> None:
    with path.open("a", encoding="utf-8") as output:
        output.write(f"{role}:{os.getpid()}\n")
        output.flush()


parser = argparse.ArgumentParser()
parser.add_argument("--pid-file", required=True)
parser.add_argument("--child", action="store_true")
parser.add_argument("--seconds", type=float, default=120.0)
args = parser.parse_args()
pid_file = pathlib.Path(args.pid_file)

if args.child:
    write_pid(pid_file, "child")
    time.sleep(args.seconds)
else:
    write_pid(pid_file, "parent")
    child = subprocess.Popen(
        [sys.executable, __file__, "--child", "--pid-file", str(pid_file), "--seconds", str(args.seconds)]
    )
    write_pid(pid_file, "spawned-child")
    try:
        time.sleep(args.seconds)
    finally:
        if child.poll() is None:
            child.terminate()
