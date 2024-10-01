import http.server
import socketserver
import os
from http.server import SimpleHTTPRequestHandler, HTTPServer

class CORSRequestHandler(SimpleHTTPRequestHandler):
    def end_headers(self):
        # Add CORS headers
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'GET')
        self.send_header('Access-Control-Allow-Headers', 'Content-Type')
        super().end_headers()

# Run the server on port 8000
if __name__ == '__main__':
    server_address = ('', 8000)
    httpd = HTTPServer(server_address, CORSRequestHandler)
    print("Serving on port 8000...")
    httpd.serve_forever()



# Define the port to serve on
PORT = 8000

# Specify the directory where your JSON file is located
# This will serve all files from this directory
DIRECTORY = os.path.dirname(os.path.abspath(__file__))  # Serve from the current directory

class MyHttpRequestHandler(http.server.SimpleHTTPRequestHandler):
    def do_GET(self):
        # Serve the files in the specified directory
        if self.path == '/JSON':
            self.path = '/output.json'  # Change this to the name of your JSON file
        return http.server.SimpleHTTPRequestHandler.do_GET(self)

# Set up the HTTP server
handler = MyHttpRequestHandler
os.chdir(DIRECTORY)

with socketserver.TCPServer(("", PORT), handler) as httpd:
    print(f"Serving JSON on port {PORT}")
    print(f"Access it at http://localhost:{PORT}/output.json")
    print("Press Ctrl+C to stop the server.")
    httpd.serve_forever()
