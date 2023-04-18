Here is some command to generate code from proto
C++:
- protoc --proto_path=. --cpp_out=. DEMO.proto

C#:
- protoc --proto_path=. --csharp_out=. --csharp_opt=serializable DEMO.proto 

JS:
- pbjs -t static-module -w commonjs --keep-case -o bundle.js  ./proto/DEMO.proto
- pbts -o bundle.d.ts bundle.js
