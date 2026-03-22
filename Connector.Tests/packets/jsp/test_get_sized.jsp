<%--
  Test: Returns exactly N bytes of binary data, where N is the "size" URL parameter.
  Usage: test_get_sized.jsp?size=1 through ?size=15
  Body content: sequential pattern 0x01..0xFE repeating (same as other binary tests)
  
  This tests every padding boundary for sizes 1-15:
    Size 1:  body 1B  → padding 3 → wire payload 7B  → Length-4=3 (GAINS 2 garbage bytes)
    Size 2:  body 2B  → padding 2 → wire payload 8B  → Length-4=4 (GAINS 2 garbage bytes)
    Size 3:  body 3B  → padding 1 → wire payload 8B  → Length-4=4 (GAINS 1 garbage byte)
    Size 4:  body 4B  → padding 0 → wire payload 8B  → Length-4=4 (CORRECT by accident)
    Size 5:  body 5B  → padding 3 → wire payload 11B → Length-4=7 (GAINS 2 garbage bytes)
    Size 6:  body 6B  → padding 2 → wire payload 11B → Length-4=7 (GAINS 1 garbage byte)
    Size 7:  body 7B  → padding 1 → wire payload 11B → Length-4=7 (CORRECT by accident)
    Size 8:  body 8B  → padding 0 → wire payload 11B → Length-4=7 (DROPS 1 byte)
    Size 9:  body 9B  → padding 3 → wire payload 15B → Length-4=11 (GAINS 2 garbage bytes)
    Size 10: body 10B → padding 2 → wire payload 15B → Length-4=11 (GAINS 1 garbage byte)
    Size 11: body 11B → padding 1 → wire payload 15B → Length-4=11 (CORRECT by accident)
    Size 12: body 12B → padding 0 → wire payload 15B → Length-4=11 (DROPS 1 byte)
    Size 13: body 13B → padding 3 → wire payload 19B → Length-4=15 (GAINS 2 garbage bytes)
    Size 14: body 14B → padding 2 → wire payload 19B → Length-4=15 (GAINS 1 garbage byte)
    Size 15: body 15B → padding 1 → wire payload 19B → Length-4=15 (CORRECT by accident)
--%><%@ page contentType="application/octet-stream" buffer="none" %><%
    String sizeParam = request.getParameter("size");
    int size = 1;
    try {
        if (sizeParam != null) size = Integer.parseInt(sizeParam);
    } catch (NumberFormatException e) {}
    if (size < 1) size = 1;
    if (size > 65535) size = 65535;
    byte[] data = new byte[size];
    for (int i = 0; i < size; i++) {
        data[i] = (byte)((i % 254) + 1);
    }
    response.getOutputStream().write(data);
    response.getOutputStream().flush();
%>
