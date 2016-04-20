import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class DataClient {
    private static  Socket socket;
    private static PrintWriter out;
    private static BufferedReader in;

    private static Float standing;

    public static void start(Float stnd) throws IOException {
        standing = stnd;
        socket = new Socket("localhost", 30001);
        out = new PrintWriter(socket.getOutputStream(), true);
        in = new BufferedReader(
                new InputStreamReader(socket.getInputStream()));
        System.out.println(socket.isConnected());
        new Thread(new ReadStreamAsyncTask()).start();
    }

    public static void send(char[] data) {
        out.println(data);
    }

    public static void send(String data) {
        out.println(data);
    }

    private static class ReadStreamAsyncTask implements Runnable {
        boolean running = true;

        @Override
        public void run() {
            String line;
            try {
                while (running) {
                    line = in.readLine();
                    standing = Float.parseFloat(line);
                    Thread.sleep(10);
                }
            } catch (IOException|InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}
