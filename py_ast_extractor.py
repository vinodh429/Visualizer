# py_ast_extractor.py
# Converts a Python source file into JSON describing:
# classes, functions, methods, call expressions

import ast
import json
import sys

class ASTExtractor(ast.NodeVisitor):
    def __init__(self):
        self.classes = []
        self.functions = []

    def visit_ClassDef(self, node):
        methods = []

        for body_item in node.body:
            if isinstance(body_item, ast.FunctionDef):
                calls = self.extract_calls(body_item)
                methods.append({
                    "name": body_item.name,
                    "calls": calls
                })

        self.classes.append({
            "name": node.name,
            "methods": methods
        })

    def visit_FunctionDef(self, node):
        calls = self.extract_calls(node)
        self.functions.append({
            "name": node.name,
            "calls": calls
        })

    def extract_calls(self, node):
        calls = []
        for child in ast.walk(node):
            if isinstance(child, ast.Call):
                try:
                    if isinstance(child.func, ast.Attribute):
                        calls.append(child.func.attr)
                    elif isinstance(child.func, ast.Name):
                        calls.append(child.func.id)
                except:
                    pass
        return calls

def main():
    if len(sys.argv) < 2:
        print("Usage: py_ast_extractor.py <python_file>")
        sys.exit(1)

    file = sys.argv[1]

    with open(file, "r", encoding="utf-8") as f:
        source = f.read()

    tree = ast.parse(source)

    extractor = ASTExtractor()
    extractor.visit(tree)

    output = {
        "file": file,
        "classes": extractor.classes,
        "functions": extractor.functions
    }

    print(json.dumps(output))

if __name__ == "__main__":
    main()
