/*
 * Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */
using System.Collections.Generic;
using System.IO;

namespace Amazon.Lambda.RuntimeSupport
{
    /// <summary>
    /// Class that contains http information for lambda response streaming along with
    /// .net stream to be used as the response.
    /// </summary>
    public class StreamedResponse
    {
        [System.Text.Json.Serialization.JsonIgnore]
        internal Stream Stream { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamedResponse"/> with
        /// default content type of "application/json".
        /// </summary>
        /// <param name="stream">Stream to be used as the lambda repsonse</param>
        public StreamedResponse(Stream stream)
        {
            Stream = stream;
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamedResponse"/> with
        /// specific http content type
        /// </summary>
        /// <param name="stream">Stream to be used as the lambda repsonse</param>
        /// <param name="contentType">HTTP Content-Type header value</param>
        public StreamedResponse(Stream stream, string contentType)
        {
            Stream = stream;
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", contentType }
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamedResponse"/> with
        /// specific http status code content type 
        /// </summary>
        /// <param name="stream">Stream to be used as the lambda repsonse</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="contentType">HTTP Content-Type header value</param>
        public StreamedResponse(Stream stream, int statusCode, string contentType)
        {
            Stream = stream;
            StatusCode = statusCode;
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", contentType }
            };
        }

        /// <summary>
        /// Cookies to be added to the http streaming response
        /// Format should be "cookieName=cookieValue"
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("cookies")]
        public List<string> Cookies { get; set; }

        /// <summary>
        /// Headers to be added to the http streaming response
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Status code to be used for http repsonse
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("statusCode")]
        public int StatusCode { get; set; } = 200;
    }
    
}
